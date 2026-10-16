using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace SupportDesk.Domain.Tickets;

public sealed class TicketMessage : Entity<Guid>
{
    public const int ContentMaxLength = 4000;
    public const int AttachmentUrlMaxLength = 1000;

    public Guid TicketId { get; private set; }

    public Guid SenderUserId { get; private set; }

    public bool IsFromStaff { get; private set; }

    public string Content { get; private set; } = null!;

    public string? AttachmentUrl { get; private set; }

    public DateTime SentAtUtc { get; private set; }

    private TicketMessage()
    {
    }

    internal TicketMessage(
        Guid id,
        Guid ticketId,
        Guid senderUserId,
        bool isFromStaff,
        string content,
        string? attachmentUrl,
        DateTime sentAtUtc)
        : base(id)
    {
        if (ticketId == Guid.Empty)
        {
            throw new DomainException("TicketMessage.TicketRequired");
        }

        if (senderUserId == Guid.Empty)
        {
            throw new DomainException("TicketMessage.SenderRequired");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new DomainException("TicketMessage.ContentRequired");
        }

        var trimmed = content.Trim();
        if (trimmed.Length > ContentMaxLength)
        {
            throw new DomainException("TicketMessage.ContentTooLong");
        }

        TicketId = ticketId;
        SenderUserId = senderUserId;
        IsFromStaff = isFromStaff;
        Content = trimmed;
        AttachmentUrl = attachmentUrl?.Trim();
        SentAtUtc = sentAtUtc;
    }
}
