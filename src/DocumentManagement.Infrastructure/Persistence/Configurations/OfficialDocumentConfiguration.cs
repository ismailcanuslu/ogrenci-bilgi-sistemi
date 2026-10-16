using DocumentManagement.Domain.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocumentManagement.Infrastructure.Persistence.Configurations;

public sealed class OfficialDocumentConfiguration : IEntityTypeConfiguration<OfficialDocument>
{
    public void Configure(EntityTypeBuilder<OfficialDocument> builder)
    {
        builder.ToTable("OfficialDocuments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Subject)
            .IsRequired()
            .HasMaxLength(OfficialDocument.SubjectMaxLength);

        builder.Property(x => x.ContentSummary)
            .IsRequired()
            .HasMaxLength(OfficialDocument.ContentSummaryMaxLength);

        builder.Property(x => x.DocumentNumber)
            .HasMaxLength(OfficialDocument.DocumentNumberMaxLength);

        builder.Property(x => x.VerificationCode)
            .IsRequired()
            .HasMaxLength(OfficialDocument.VerificationCodeMaxLength);

        builder.Property(x => x.VerificationHash)
            .IsRequired()
            .HasMaxLength(OfficialDocument.VerificationHashMaxLength);

        builder.Property(x => x.DepartmentCode)
            .HasMaxLength(OfficialDocument.DepartmentCodeMaxLength);

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.HasIndex(x => x.VerificationCode).IsUnique();
        builder.HasIndex(x => x.DocumentNumber).IsUnique().HasFilter("[DocumentNumber] IS NOT NULL");

        builder.HasMany(x => x.WorkflowSteps)
            .WithOne()
            .HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
