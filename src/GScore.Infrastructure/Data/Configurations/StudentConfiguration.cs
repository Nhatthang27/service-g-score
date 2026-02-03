using GScore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GScore.Infrastructure.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<StudentEntity>
{
    public void Configure(EntityTypeBuilder<StudentEntity> builder)
    {
        builder.ToTable("students");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.RegistrationNumber)
            .HasColumnName("registration_number")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ForeignLanguageCode)
            .HasColumnName("foreign_language_code")
            .HasMaxLength(10);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(x => x.DeletedAt)
            .HasColumnName("deleted_at");

        builder.HasMany(x => x.Scores)
            .WithOne(x => x.Student)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.RegistrationNumber)
            .HasDatabaseName("ix_students_registration_number")
            .IsUnique();

        builder.HasQueryFilter(x => x.DeletedAt == null);
    }
}
