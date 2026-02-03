using GScore.Domain.Constants;
using GScore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GScore.Infrastructure.Data.Configurations;

public class CodeSessionConfiguration : IEntityTypeConfiguration<CodeSessionEntity>
{
    public void Configure(EntityTypeBuilder<CodeSessionEntity> builder)
    {
        builder.ToTable("code_sessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

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
                v => Enum.Parse<SessionStatus>(v, true))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(x => x.DeletedAt)
            .HasColumnName("deleted_at");

        builder.HasMany(x => x.Executions)
            .WithOne(x => x.Session)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_code_sessions_status");

        builder.HasQueryFilter(x => x.DeletedAt == null);
    }
}
