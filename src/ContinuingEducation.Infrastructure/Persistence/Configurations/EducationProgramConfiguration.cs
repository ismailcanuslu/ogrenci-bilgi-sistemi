using ContinuingEducation.Domain.Programs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContinuingEducation.Infrastructure.Persistence.Configurations;

public sealed class EducationProgramConfiguration : IEntityTypeConfiguration<EducationProgram>
{
    public void Configure(EntityTypeBuilder<EducationProgram> builder)
    {
        builder.ToTable("EducationPrograms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(EducationProgram.TitleMaxLength);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(EducationProgram.CodeMaxLength);

        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.Description)
            .HasMaxLength(EducationProgram.DescriptionMaxLength);

        builder.Property(x => x.InstructorUserId)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.MinPassingGrade)
            .HasColumnType("decimal(5,2)");

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);
    }
}
