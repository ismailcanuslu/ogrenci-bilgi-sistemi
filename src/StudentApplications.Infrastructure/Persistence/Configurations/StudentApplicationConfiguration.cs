using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentApplications.Domain.Applications;

namespace StudentApplications.Infrastructure.Persistence.Configurations;

public sealed class StudentApplicationConfiguration : IEntityTypeConfiguration<StudentApplication>
{
    public void Configure(EntityTypeBuilder<StudentApplication> builder)
    {
        builder.ToTable("StudentApplications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ApplicationNumber)
            .IsRequired()
            .HasMaxLength(StudentApplication.ApplicationNumberMaxLength);

        builder.Property(x => x.StudentId)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(x => x.State)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(x => x.TargetInstitution)
            .HasMaxLength(StudentApplication.TargetInstitutionMaxLength);

        builder.Property(x => x.Details)
            .IsRequired()
            .HasMaxLength(StudentApplication.DetailsMaxLength);

        builder.Property(x => x.ReviewNotes)
            .HasMaxLength(StudentApplication.ReviewNotesMaxLength);

        builder.Property(x => x.Gpa)
            .HasColumnType("decimal(4,2)");

        builder.HasIndex(x => x.ApplicationNumber).IsUnique();
        builder.HasIndex(x => x.StudentId);
    }
}
