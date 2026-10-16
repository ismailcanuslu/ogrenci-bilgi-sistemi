using Academic.Domain.Departments;
using Academic.Domain.Faculties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academic.Infrastructure.Persistence.Configurations;

public sealed class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
{
    public void Configure(EntityTypeBuilder<Faculty> builder)
    {
        builder.ToTable("faculties");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Code).IsRequired().HasMaxLength(Faculty.CodeMaxLength);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(Faculty.NameMaxLength);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc);

        builder.HasIndex(x => x.Code).IsUnique();
    }
}

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.FacultyId).IsRequired();
        builder.Property(x => x.Code).IsRequired().HasMaxLength(Department.CodeMaxLength);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(Department.NameMaxLength);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc);

        // Geçmiş akademik kayıtların bağlı olduğu fakülte yanlışlıkla silinemez.
        builder.HasOne<Faculty>()
            .WithMany()
            .HasForeignKey(x => x.FacultyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.FacultyId, x.Code }).IsUnique();
    }
}
