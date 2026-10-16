using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportDesk.Domain.Tickets;

namespace SupportDesk.Infrastructure.Persistence.Configurations;

public sealed class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.ToTable("support_tickets", "support_desk");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TicketNumber)
            .HasMaxLength(SupportTicket.TicketNumberMaxLength)
            .IsRequired();

        builder.Property(x => x.RequesterUserId).IsRequired();
        builder.Property(x => x.TargetUnit).IsRequired();
        builder.Property(x => x.Subject)
            .HasMaxLength(SupportTicket.SubjectMaxLength)
            .IsRequired();
        builder.Property(x => x.Priority).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.AssignedStaffUserId);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc);
        builder.Property(x => x.ClosedAtUtc);

        builder.HasIndex(x => x.TicketNumber).IsUnique();
        builder.HasIndex(x => x.RequesterUserId);
        builder.HasIndex(x => new { x.TargetUnit, x.Status });

        builder.HasMany(x => x.Messages)
            .WithOne()
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Messages)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
