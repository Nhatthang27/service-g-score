using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using GScore.Application.DTOs;
using GScore.Application.Interfaces;
using GScore.Domain.Constants;
using GScore.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;

namespace GScore.Infrastructure.Services;

public class CsvImportService : ICsvImportService
{
    private readonly string _connectionString;
    private const int ProgressInterval = 100_000;

    public CsvImportService(IOptions<PostgresSetting> settings)
    {
        _connectionString = settings.Value.ConnectionString;
    }

    public async Task<ImportResultDto> ImportStudentsFromCsvAsync(Stream csvStream, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var totalRecords = 0;
        var successCount = 0;
        var failedCount = 0;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        // Pre-import: Clear data and optimize tables
        await ExecutePreImportAsync(connection, cancellationToken);

        // Collect exam scores to write after students
        var examScores = new List<(Guid StudentId, SubjectType Subject, decimal? Score, DateTimeOffset CreatedAt)>();

        try
        {
            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                BadDataFound = null,
                TrimOptions = TrimOptions.Trim
            });

            await csv.ReadAsync();
            csv.ReadHeader();

            var now = DateTimeOffset.UtcNow;

            // Phase 1: Write students using binary COPY
            await using (var studentWriter = await connection.BeginBinaryImportAsync(
                "COPY students (id, registration_number, foreign_language_code, created_at) FROM STDIN (FORMAT BINARY)",
                cancellationToken))
            {
                while (await csv.ReadAsync())
                {
                    totalRecords++;
                    try
                    {
                        var studentId = Guid.NewGuid();
                        var registrationNumber = csv.GetField<string>("sbd") ?? string.Empty;
                        var foreignLanguageCode = GetNullableString(csv, "ma_ngoai_ngu");

                        // Write student row
                        await studentWriter.StartRowAsync(cancellationToken);
                        await studentWriter.WriteAsync(studentId, NpgsqlDbType.Uuid, cancellationToken);
                        await studentWriter.WriteAsync(registrationNumber, NpgsqlDbType.Text, cancellationToken);

                        if (foreignLanguageCode != null)
                            await studentWriter.WriteAsync(foreignLanguageCode, NpgsqlDbType.Text, cancellationToken);
                        else
                            await studentWriter.WriteNullAsync(cancellationToken);

                        await studentWriter.WriteAsync(now, NpgsqlDbType.TimestampTz, cancellationToken);

                        // Collect exam scores for phase 2
                        CollectScore(examScores, studentId, SubjectType.Toan, csv, "toan", now);
                        CollectScore(examScores, studentId, SubjectType.NguVan, csv, "ngu_van", now);
                        CollectScore(examScores, studentId, SubjectType.NgoaiNgu, csv, "ngoai_ngu", now);
                        CollectScore(examScores, studentId, SubjectType.VatLi, csv, "vat_li", now);
                        CollectScore(examScores, studentId, SubjectType.HoaHoc, csv, "hoa_hoc", now);
                        CollectScore(examScores, studentId, SubjectType.SinhHoc, csv, "sinh_hoc", now);
                        CollectScore(examScores, studentId, SubjectType.LichSu, csv, "lich_su", now);
                        CollectScore(examScores, studentId, SubjectType.DiaLi, csv, "dia_li", now);
                        CollectScore(examScores, studentId, SubjectType.GDCD, csv, "gdcd", now);

                        successCount++;

                        if (totalRecords % ProgressInterval == 0)
                        {
                            Console.WriteLine($"[CSV Import] Processed {totalRecords:N0} student rows...");
                        }
                    }
                    catch
                    {
                        failedCount++;
                    }
                }

                // Complete students COPY
                await studentWriter.CompleteAsync(cancellationToken);
            } // studentWriter is disposed here, connection is freed

            Console.WriteLine($"[CSV Import] Students COPY completed. Total: {successCount:N0}");

            // Phase 2: Write exam scores using binary COPY (connection is now free)
            await WriteExamScoresAsync(connection, examScores, cancellationToken);
        }
        finally
        {
            // Post-import: Recreate indexes and restore settings
            await ExecutePostImportAsync(connection, cancellationToken);
        }

        stopwatch.Stop();

        Console.WriteLine($"[CSV Import] Completed in {stopwatch.ElapsedMilliseconds}ms. Success: {successCount:N0}, Failed: {failedCount:N0}");

        return new ImportResultDto(
            totalRecords,
            successCount,
            failedCount,
            stopwatch.ElapsedMilliseconds);
    }

    private static async Task WriteExamScoresAsync(
        NpgsqlConnection connection,
        List<(Guid StudentId, SubjectType Subject, decimal? Score, DateTimeOffset CreatedAt)> examScores,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"[CSV Import] Writing {examScores.Count:N0} exam scores...");

        await using var scoreWriter = await connection.BeginBinaryImportAsync(
            "COPY exam_scores (id, student_id, subject, score, created_at) FROM STDIN (FORMAT BINARY)",
            cancellationToken);

        var count = 0;
        foreach (var (studentId, subject, score, createdAt) in examScores)
        {
            await scoreWriter.StartRowAsync(cancellationToken);
            await scoreWriter.WriteAsync(Guid.NewGuid(), NpgsqlDbType.Uuid, cancellationToken);
            await scoreWriter.WriteAsync(studentId, NpgsqlDbType.Uuid, cancellationToken);
            await scoreWriter.WriteAsync(subject.ToString().ToUpperInvariant(), NpgsqlDbType.Text, cancellationToken);

            if (score.HasValue)
                await scoreWriter.WriteAsync(score.Value, NpgsqlDbType.Numeric, cancellationToken);
            else
                await scoreWriter.WriteNullAsync(cancellationToken);

            await scoreWriter.WriteAsync(createdAt, NpgsqlDbType.TimestampTz, cancellationToken);

            count++;
            if (count % 500_000 == 0)
            {
                Console.WriteLine($"[CSV Import] Written {count:N0} exam score rows...");
            }
        }

        await scoreWriter.CompleteAsync(cancellationToken);
        Console.WriteLine($"[CSV Import] Exam scores COPY completed. Total: {count:N0}");
    }

    private static void CollectScore(
        List<(Guid StudentId, SubjectType Subject, decimal? Score, DateTimeOffset CreatedAt)> scores,
        Guid studentId,
        SubjectType subject,
        CsvReader csv,
        string fieldName,
        DateTimeOffset createdAt)
    {
        var scoreStr = csv.GetField<string>(fieldName);
        decimal? score = null;

        if (!string.IsNullOrWhiteSpace(scoreStr) &&
            decimal.TryParse(scoreStr, CultureInfo.InvariantCulture, out var parsedScore))
        {
            score = parsedScore;
        }

        scores.Add((studentId, subject, score, createdAt));
    }

    private static string? GetNullableString(CsvReader csv, string fieldName)
    {
        var value = csv.GetField<string>(fieldName);
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static async Task ExecutePreImportAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        Console.WriteLine("[CSV Import] Starting pre-import optimizations...");

        var commands = new[]
        {
            // Clear existing data (TRUNCATE is faster than DELETE)
            "TRUNCATE TABLE exam_scores, students RESTART IDENTITY CASCADE",

            // Set tables to UNLOGGED (no WAL = faster writes)
            "ALTER TABLE students SET UNLOGGED",
            "ALTER TABLE exam_scores SET UNLOGGED",

            // Drop indexes for faster inserts
            "DROP INDEX IF EXISTS ix_students_registration_number",
            "DROP INDEX IF EXISTS ix_exam_scores_student_id",
            "DROP INDEX IF EXISTS ix_exam_scores_subject",
            "DROP INDEX IF EXISTS ix_exam_scores_student_subject",

            // Disable triggers
            "ALTER TABLE students DISABLE TRIGGER ALL",
            "ALTER TABLE exam_scores DISABLE TRIGGER ALL"
        };

        foreach (var sql in commands)
        {
            try
            {
                await using var cmd = new NpgsqlCommand(sql, connection);
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CSV Import] Pre-import warning: {ex.Message}");
            }
        }

        Console.WriteLine("[CSV Import] Pre-import optimizations completed.");
    }

    private static async Task ExecutePostImportAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        Console.WriteLine("[CSV Import] Starting post-import optimizations...");

        var commands = new[]
        {
            // Set tables back to LOGGED
            "ALTER TABLE students SET LOGGED",
            "ALTER TABLE exam_scores SET LOGGED",

            // Re-enable triggers
            "ALTER TABLE students ENABLE TRIGGER ALL",
            "ALTER TABLE exam_scores ENABLE TRIGGER ALL",

            // Recreate indexes
            "CREATE UNIQUE INDEX ix_students_registration_number ON students(registration_number)",
            "CREATE INDEX ix_exam_scores_student_id ON exam_scores(student_id)",
            "CREATE INDEX ix_exam_scores_subject ON exam_scores(subject)",
            "CREATE UNIQUE INDEX ix_exam_scores_student_subject ON exam_scores(student_id, subject)",

            // Analyze tables for query optimizer
            "ANALYZE students",
            "ANALYZE exam_scores"
        };

        foreach (var sql in commands)
        {
            try
            {
                await using var cmd = new NpgsqlCommand(sql, connection)
                {
                    CommandTimeout = 3600 // 1 hour for index creation on large tables
                };
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CSV Import] Post-import warning: {ex.Message}");
            }
        }

        Console.WriteLine("[CSV Import] Post-import optimizations completed.");
    }
}
