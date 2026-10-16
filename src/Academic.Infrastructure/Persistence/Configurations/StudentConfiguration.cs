using Academic.Domain.Curriculums;
using Academic.Domain.Departments;
using Academic.Domain.Programs;
using Academic.Domain.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academic.Infrastructure.Persistence.Configurations;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("students", "academic");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StudentNumber)
            .HasMaxLength(Student.StudentNumberMaxLength)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasMaxLength(Student.NameMaxLength)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(Student.NameMaxLength)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(Student.EmailMaxLength)
            .IsRequired();

        builder.Property(x => x.DepartmentId).IsRequired();
        builder.Property(x => x.ProgramId).IsRequired();
        builder.Property(x => x.CurriculumVersionId).IsRequired();
        builder.Property(x => x.EnrollmentYear).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.EnrollmentDateUtc).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc);

        builder.HasIndex(x => x.StudentNumber).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.DepartmentId);
        builder.HasIndex(x => x.ProgramId);
        builder.HasIndex(x => x.UserId);

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

        builder.HasMany(x => x.ProgramEnrollments)
            .WithOne()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.ProgramEnrollments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
