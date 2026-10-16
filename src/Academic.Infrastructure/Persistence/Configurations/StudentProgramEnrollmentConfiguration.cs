using Academic.Domain.Curriculums;
using Academic.Domain.Departments;
using Academic.Domain.Programs;
using Academic.Domain.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academic.Infrastructure.Persistence.Configurations;

public sealed class StudentProgramEnrollmentConfiguration : IEntityTypeConfiguration<StudentProgramEnrollment>
{
    public void Configure(EntityTypeBuilder<StudentProgramEnrollment> builder)
    {
        builder.ToTable("student_program_enrollments", "academic");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StudentId).IsRequired();
        builder.Property(x => x.DepartmentId).IsRequired();
        builder.Property(x => x.ProgramId).IsRequired();
        builder.Property(x => x.CurriculumVersionId).IsRequired();
        builder.Property(x => x.EnrollmentType).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.EnrollmentYear).IsRequired();
        builder.Property(x => x.EnrollmentDateUtc).IsRequired();
        builder.Property(x => x.CompletionDateUtc);
        builder.Property(x => x.IsPrimary).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(500);

        builder.HasIndex(x => x.StudentId);
        builder.HasIndex(x => x.DepartmentId);
        builder.HasIndex(x => x.ProgramId);
        builder.HasIndex(x => new { x.StudentId, x.ProgramId });

        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Program>()
            .WithMany()
            .HasForeignKey(x => x.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<CurriculumVersion>()
            .WithMany()
            .HasForeignKey(x => x.CurriculumVersionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
