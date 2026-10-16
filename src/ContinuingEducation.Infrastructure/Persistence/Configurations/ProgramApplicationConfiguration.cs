using ContinuingEducation.Domain.Programs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContinuingEducation.Infrastructure.Persistence.Configurations;

public sealed class ProgramApplicationConfiguration : IEntityTypeConfiguration<ProgramApplication>
{
    public void Configure(EntityTypeBuilder<ProgramApplication> builder)
    {
        builder.ToTable("ProgramApplications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProgramId)
            .IsRequired();

        builder.Property(x => x.ApplicantUserId)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.FinalGrade)
            .HasColumnType("decimal(5,2)");

        builder.Property(x => x.CertificateNumber)
            .HasMaxLength(ProgramApplication.CertificateNumberMaxLength);

        builder.HasIndex(x => new { x.ProgramId, x.ApplicantUserId }).IsUnique();
        builder.HasIndex(x => x.CertificateNumber).IsUnique().HasFilter("[CertificateNumber] IS NOT NULL");
    }
}
