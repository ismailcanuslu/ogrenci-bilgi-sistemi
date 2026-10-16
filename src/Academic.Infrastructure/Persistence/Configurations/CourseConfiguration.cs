using Academic.Domain.Courses;
using Academic.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academic.Infrastructure.Persistence.Configurations;

public sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("courses", "academic");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DepartmentId).IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(Course.CodeMaxLength)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(Course.NameMaxLength)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(Course.DescriptionMaxLength);

        builder.Property(x => x.DefaultEcts).IsRequired();
        builder.Property(x => x.DefaultCredit).IsRequired();

        builder.Property(x => x.Prerequisites)
            .HasMaxLength(Course.PrerequisitesMaxLength);

        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc);

        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.DepartmentId);

        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
