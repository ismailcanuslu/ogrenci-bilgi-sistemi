using DocumentManagement.Domain.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocumentManagement.Infrastructure.Persistence.Configurations;

public sealed class DocumentWorkflowStepConfiguration : IEntityTypeConfiguration<DocumentWorkflowStep>
{
    public void Configure(EntityTypeBuilder<DocumentWorkflowStep> builder)
    {
        builder.ToTable("DocumentWorkflowSteps");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentId)
            .IsRequired();

        builder.Property(x => x.AssigneeUserId)
            .IsRequired();

        builder.Property(x => x.StepType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.Note)
            .HasMaxLength(DocumentWorkflowStep.NoteMaxLength);

        builder.HasIndex(x => new { x.DocumentId, x.StepOrder }).IsUnique();
    }
}
