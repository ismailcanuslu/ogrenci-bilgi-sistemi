using Academic.Domain.AcademicYears;
using Academic.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academic.Infrastructure.Persistence.Configurations;

public sealed class CourseOfferingConfiguration : IEntityTypeConfiguration<CourseOffering>
{
    public void Configure(EntityTypeBuilder<CourseOffering> builder)
    {
        builder.ToTable("course_offerings", "academic");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CourseId).IsRequired();
        builder.Property(x => x.AcademicTermId).IsRequired();
        builder.Property(x => x.Section).IsRequired();
        builder.Property(x => x.Capacity).IsRequired();
        builder.Property(x => x.EnrolledCount).IsRequired();
        builder.Property(x => x.IsRegistrationOpen).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc);

        builder.Ignore(x => x.PrimaryInstructorId);

        builder.HasIndex(x => new { x.CourseId, x.AcademicTermId, x.Section }).IsUnique();
        builder.HasIndex(x => x.AcademicTermId);

        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AcademicTerm>()
            .WithMany()
            .HasForeignKey(x => x.AcademicTermId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Instructors)
            .WithOne()
            .HasForeignKey(x => x.CourseOfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Instructors)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Schedules)
            .WithOne()
            .HasForeignKey(x => x.CourseOfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Schedules)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class CourseOfferingInstructorConfiguration : IEntityTypeConfiguration<CourseOfferingInstructor>
{
    public void Configure(EntityTypeBuilder<CourseOfferingInstructor> builder)
    {
        builder.ToTable("course_offering_instructors", "academic");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CourseOfferingId).IsRequired();
        builder.Property(x => x.InstructorId).IsRequired();
        builder.Property(x => x.IsPrimary).IsRequired();
        builder.Property(x => x.RoleOrTitle).HasMaxLength(CourseOfferingInstructor.RoleOrTitleMaxLength);
        builder.Property(x => x.AssignedAtUtc).IsRequired();

        builder.HasIndex(x => new { x.CourseOfferingId, x.InstructorId }).IsUnique();
        builder.HasIndex(x => x.InstructorId);
    }
}

public sealed class CourseOfferingScheduleConfiguration : IEntityTypeConfiguration<CourseOfferingSchedule>
{
    public void Configure(EntityTypeBuilder<CourseOfferingSchedule> builder)
    {
        builder.ToTable("course_offering_schedules", "academic");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CourseOfferingId).IsRequired();
        builder.Property(x => x.SessionType).IsRequired();

        builder.Property(x => x.BuildingOrFaculty)
            .HasMaxLength(CourseOfferingSchedule.BuildingOrFacultyMaxLength)
            .IsRequired();

        builder.Property(x => x.Classroom)
            .HasMaxLength(CourseOfferingSchedule.ClassroomMaxLength)
            .IsRequired();

        builder.Property(x => x.DayOfWeek).IsRequired();
        builder.Property(x => x.StartTime).IsRequired();
        builder.Property(x => x.EndTime).IsRequired();
        builder.Property(x => x.InstructorId);
        builder.Property(x => x.IsOnline).IsRequired();

        builder.HasIndex(x => x.CourseOfferingId);
        builder.HasIndex(x => x.InstructorId);
        builder.HasIndex(x => new { x.BuildingOrFaculty, x.Classroom, x.DayOfWeek, x.StartTime, x.EndTime });
    }
}
