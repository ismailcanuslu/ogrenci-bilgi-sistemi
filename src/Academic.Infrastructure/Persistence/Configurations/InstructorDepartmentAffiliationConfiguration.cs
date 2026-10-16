using Academic.Domain.Departments;
using Academic.Domain.Faculties;
using Academic.Domain.Instructors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academic.Infrastructure.Persistence.Configurations;

public sealed class InstructorDepartmentAffiliationConfiguration
    : IEntityTypeConfiguration<InstructorDepartmentAffiliation>
{
    public void Configure(EntityTypeBuilder<InstructorDepartmentAffiliation> builder)
    {
        builder.ToTable("instructor_department_affiliations", "academic");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.InstructorId).IsRequired();
        builder.Property(x => x.FacultyId).IsRequired();
        builder.Property(x => x.DepartmentId).IsRequired();
        builder.Property(x => x.AffiliationType).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(InstructorDepartmentAffiliation.TitleMaxLength);
        builder.Property(x => x.IsPrimary).IsRequired();
        builder.Property(x => x.StartDateUtc).IsRequired();
        builder.Property(x => x.EndDateUtc);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc);

        builder.HasIndex(x => x.InstructorId);
        builder.HasIndex(x => x.DepartmentId);
        builder.HasIndex(x => x.FacultyId);
        builder.HasIndex(x => new { x.InstructorId, x.DepartmentId });

        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Faculty>()
            .WithMany()
            .HasForeignKey(x => x.FacultyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
