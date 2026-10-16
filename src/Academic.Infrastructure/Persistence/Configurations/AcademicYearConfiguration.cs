using Academic.Domain.AcademicYears;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academic.Infrastructure.Persistence.Configurations;

public sealed class AcademicYearConfiguration : IEntityTypeConfiguration<AcademicYear>
{
    public void Configure(EntityTypeBuilder<AcademicYear> builder)
    {
        builder.ToTable("academic_years");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Code).IsRequired().HasMaxLength(AcademicYear.CodeMaxLength);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(AcademicYear.NameMaxLength);
        builder.Property(x => x.StartDate).IsRequired();
        builder.Property(x => x.EndDate).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc);

        builder.HasIndex(x => x.Code).IsUnique();

        builder.HasMany(x => x.Terms)
            .WithOne()
            .HasForeignKey(x => x.AcademicYearId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Terms)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class AcademicTermConfiguration : IEntityTypeConfiguration<AcademicTerm>
{
    public void Configure(EntityTypeBuilder<AcademicTerm> builder)
    {
        builder.ToTable("academic_terms");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.AcademicYearId).IsRequired();
        builder.Property(x => x.TermType).IsRequired().HasConversion<int>();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(AcademicTerm.NameMaxLength);
        builder.Property(x => x.StartDate).IsRequired();
        builder.Property(x => x.EndDate).IsRequired();
        builder.Property(x => x.IsRegistrationOpen).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();

        builder.HasIndex(x => new { x.AcademicYearId, x.TermType }).IsUnique();
    }
}
