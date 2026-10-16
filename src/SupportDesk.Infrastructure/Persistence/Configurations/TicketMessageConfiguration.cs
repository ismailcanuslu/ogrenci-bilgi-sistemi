using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportDesk.Domain.Tickets;

namespace SupportDesk.Infrastructure.Persistence.Configurations;

public sealed class TicketMessageConfiguration : IEntityTypeConfiguration<TicketMessage>
{
    public void Configure(EntityTypeBuilder<TicketMessage> builder)
    {
        builder.ToTable("ticket_messages", "support_desk");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TicketId).IsRequired();
        builder.Property(x => x.SenderUserId).IsRequired();
        builder.Property(x => x.IsFromStaff).IsRequired();
        builder.Property(x => x.Content)
            .HasMaxLength(TicketMessage.ContentMaxLength)
            .IsRequired();
        builder.Property(x => x.AttachmentUrl).HasMaxLength(TicketMessage.AttachmentUrlMaxLength);
        builder.Property(x => x.SentAtUtc).IsRequired();

        builder.HasIndex(x => x.TicketId);
        builder.HasIndex(x => x.SenderUserId);
    }
}
