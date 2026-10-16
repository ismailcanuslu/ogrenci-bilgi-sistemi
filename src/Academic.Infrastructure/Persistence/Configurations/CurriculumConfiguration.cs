using Academic.Domain.AcademicYears;
using Academic.Domain.Curriculums;
using Academic.Domain.Programs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academic.Infrastructure.Persistence.Configurations;

public sealed class CurriculumConfiguration : IEntityTypeConfiguration<Curriculum>
{
    public void Configure(EntityTypeBuilder<Curriculum> builder)
    {
        builder.ToTable("curriculums");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ProgramId).IsRequired();
        builder.Property(x => x.AcademicYearId).IsRequired();
        builder.Property(x => x.Code).IsRequired().HasMaxLength(Curriculum.CodeMaxLength);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(Curriculum.NameMaxLength);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc);

        builder.HasOne<Program>()
            .WithMany()
            .HasForeignKey(x => x.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AcademicYear>()
            .WithMany()
            .HasForeignKey(x => x.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ProgramId, x.AcademicYearId }).IsUnique();

        builder.HasMany(x => x.Versions)
            .WithOne()
            .HasForeignKey(x => x.CurriculumId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Versions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class CurriculumVersionConfiguration : IEntityTypeConfiguration<CurriculumVersion>
{
    public void Configure(EntityTypeBuilder<CurriculumVersion> builder)
    {
        builder.ToTable("curriculum_versions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.CurriculumId).IsRequired();
        builder.Property(x => x.VersionNumber).IsRequired();
        builder.Property(x => x.Status).IsRequired().HasConversion<int>();
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.PublishedAtUtc);

        builder.Ignore(x => x.TotalEcts);

        builder.HasIndex(x => new { x.CurriculumId, x.VersionNumber }).IsUnique();

        builder.HasMany(x => x.Courses)
            .WithOne()
            .HasForeignKey(x => x.CurriculumVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Courses)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class CurriculumCourseConfiguration : IEntityTypeConfiguration<CurriculumCourse>
{
    public void Configure(EntityTypeBuilder<CurriculumCourse> builder)
    {
        builder.ToTable("curriculum_courses");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.CurriculumVersionId).IsRequired();
        builder.Property(x => x.CourseCode).IsRequired().HasMaxLength(CurriculumCourse.CodeMaxLength);
        builder.Property(x => x.CourseName).IsRequired().HasMaxLength(CurriculumCourse.NameMaxLength);
        builder.Property(x => x.Ects).IsRequired();
        builder.Property(x => x.Credit).IsRequired();
        builder.Property(x => x.Semester).IsRequired();
        builder.Property(x => x.IsElective).IsRequired();
        builder.Property(x => x.Category).IsRequired().HasConversion<int>();
        builder.Property(x => x.Prerequisites).HasMaxLength(200);

        builder.HasIndex(x => new { x.CurriculumVersionId, x.CourseCode, x.Semester }).IsUnique();
    }
}
