using Academic.Domain.Departments;
using Academic.Domain.Programs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academic.Infrastructure.Persistence.Configurations;

public sealed class ProgramConfiguration : IEntityTypeConfiguration<Program>
{
    public void Configure(EntityTypeBuilder<Program> builder)
    {
        builder.ToTable("programs");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.DepartmentId).IsRequired();
        builder.Property(x => x.Code).IsRequired().HasMaxLength(Program.CodeMaxLength);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(Program.NameMaxLength);
        builder.Property(x => x.DegreeLevel).IsRequired().HasConversion<int>();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc);

        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.DepartmentId, x.Code }).IsUnique();
    }
}
