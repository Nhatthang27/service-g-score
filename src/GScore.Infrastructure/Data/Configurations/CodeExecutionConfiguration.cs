using GScore.Domain.Constants;
using GScore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GScore.Infrastructure.Data.Configurations;

public class CodeExecutionConfiguration : IEntityTypeConfiguration<CodeExecutionEntity>
{
    public void Configure(EntityTypeBuilder<CodeExecutionEntity> builder)
    {
        builder.ToTable("code_executions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.SessionId)
            .HasColumnName("session_id")
            .IsRequired();

        builder.Property(x => x.Language)
            .HasColumnName("language")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.SourceCode)
            .HasColumnName("source_code")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<ExecutionStatus>(v, true))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Stdout)
            .HasColumnName("stdout")
            .HasColumnType("text");

        builder.Property(x => x.Stderr)
            .HasColumnName("stderr")
            .HasColumnType("text");

        builder.Property(x => x.ExecutionTimeMs)
            .HasColumnName("execution_time_ms");

        builder.Property(x => x.MemoryUsageMb)
            .HasColumnName("memory_usage_mb")
            .HasPrecision(10, 2);

        builder.Property(x => x.ExitCode)
            .HasColumnName("exit_code");

        builder.Property(x => x.ErrorMessage)
            .HasColumnName("error_message")
            .HasColumnType("text");

        builder.Property(x => x.QueuedAt)
            .HasColumnName("queued_at")
            .IsRequired();

        builder.Property(x => x.StartedAt)
            .HasColumnName("started_at");

        builder.Property(x => x.CompletedAt)
            .HasColumnName("completed_at");

        builder.Property(x => x.RetryCount)
            .HasColumnName("retry_count")
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(x => x.DeletedAt)
            .HasColumnName("deleted_at");

        builder.HasOne(x => x.Session)
            .WithMany(x => x.Executions)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.SessionId)
            .HasDatabaseName("ix_code_executions_session_id");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_code_executions_status");

        builder.HasIndex(x => x.QueuedAt)
            .HasDatabaseName("ix_code_executions_queued_at");

        builder.HasQueryFilter(x => x.DeletedAt == null);
    }
}
