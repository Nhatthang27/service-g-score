using GScore.Domain.Constants;
using GScore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GScore.Infrastructure.Data.Configurations;

public class ExamScoreConfiguration : IEntityTypeConfiguration<ExamScoreEntity>
{
    public void Configure(EntityTypeBuilder<ExamScoreEntity> builder)
    {
        builder.ToTable("exam_scores");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.StudentId)
            .HasColumnName("student_id")
            .IsRequired();

        builder.Property(x => x.Subject)
            .HasColumnName("subject")
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<SubjectType>(v, true))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Score)
            .HasColumnName("score")
            .HasPrecision(4, 2);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(x => x.DeletedAt)
            .HasColumnName("deleted_at");

        builder.HasOne(x => x.Student)
            .WithMany(x => x.Scores)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.StudentId)
            .HasDatabaseName("ix_exam_scores_student_id");

        builder.HasIndex(x => x.Subject)
            .HasDatabaseName("ix_exam_scores_subject");

        builder.HasIndex(x => new { x.StudentId, x.Subject })
            .HasDatabaseName("ix_exam_scores_student_subject")
            .IsUnique();

        builder.HasQueryFilter(x => x.DeletedAt == null);
    }
}
