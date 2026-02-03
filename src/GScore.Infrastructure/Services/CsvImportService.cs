using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using GScore.Application.DTOs;
using GScore.Application.Interfaces;
using GScore.Domain.Constants;
using GScore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GScore.Infrastructure.Services;

public class CsvImportService(IApplicationDbContext context) : ICsvImportService
{
    public async Task<ImportResultDto> ImportStudentsFromCsvAsync(Stream csvStream, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var totalRecords = 0;
        var successCount = 0;
        var failedCount = 0;

        // Clear existing data
        await context.ExamScores.ExecuteDeleteAsync(cancellationToken);
        await context.Students.ExecuteDeleteAsync(cancellationToken);

        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null
        });

        await csv.ReadAsync();
        csv.ReadHeader();

        var students = new List<StudentEntity>();

        while (await csv.ReadAsync())
        {
            totalRecords++;
            try
            {
                var student = new StudentEntity
                {
                    RegistrationNumber = csv.GetField<string>("sbd") ?? string.Empty,
                    ForeignLanguageCode = GetNullableString(csv, "ma_ngoai_ngu"),
                    Scores = new List<ExamScoreEntity>()
                };

                AddScore(student, SubjectType.Toan, csv, "toan");
                AddScore(student, SubjectType.NguVan, csv, "ngu_van");
                AddScore(student, SubjectType.NgoaiNgu, csv, "ngoai_ngu");
                AddScore(student, SubjectType.VatLi, csv, "vat_li");
                AddScore(student, SubjectType.HoaHoc, csv, "hoa_hoc");
                AddScore(student, SubjectType.SinhHoc, csv, "sinh_hoc");
                AddScore(student, SubjectType.LichSu, csv, "lich_su");
                AddScore(student, SubjectType.DiaLi, csv, "dia_li");
                AddScore(student, SubjectType.GDCD, csv, "gdcd");

                students.Add(student);
                successCount++;

                // Batch insert every 1000 records
                if (students.Count >= 1000)
                {
                    context.Students.AddRange(students);
                    await context.SaveChangesAsync(cancellationToken);
                    students.Clear();
                }
            }
            catch
            {
                failedCount++;
            }
        }

        // Insert remaining records
        if (students.Count > 0)
        {
            context.Students.AddRange(students);
            await context.SaveChangesAsync(cancellationToken);
        }

        stopwatch.Stop();

        return new ImportResultDto(
            totalRecords,
            successCount,
            failedCount,
            stopwatch.ElapsedMilliseconds);
    }

    private static string? GetNullableString(CsvReader csv, string fieldName)
    {
        var value = csv.GetField<string>(fieldName);
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static void AddScore(StudentEntity student, SubjectType subject, CsvReader csv, string fieldName)
    {
        var scoreStr = csv.GetField<string>(fieldName);
        decimal? score = null;

        if (!string.IsNullOrWhiteSpace(scoreStr) && decimal.TryParse(scoreStr, CultureInfo.InvariantCulture, out var parsedScore))
        {
            score = parsedScore;
        }

        student.Scores.Add(new ExamScoreEntity
        {
            Subject = subject,
            Score = score
        });
    }
}
