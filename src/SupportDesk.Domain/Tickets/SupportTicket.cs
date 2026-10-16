using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace SupportDesk.Domain.Tickets;

public sealed class SupportTicket : AggregateRoot<Guid>
{
    public const int TicketNumberMaxLength = 30;
    public const int SubjectMaxLength = 200;

    private readonly List<TicketMessage> _messages = [];

    public string TicketNumber { get; private set; } = null!;

    public Guid RequesterUserId { get; private set; }

    public SupportUnit TargetUnit { get; private set; }

    public string Subject { get; private set; } = null!;

    public TicketPriority Priority { get; private set; }

    public TicketStatus Status { get; private set; }

    public Guid? AssignedStaffUserId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public DateTime? ClosedAtUtc { get; private set; }

    public IReadOnlyCollection<TicketMessage> Messages => _messages.AsReadOnly();

    private SupportTicket()
    {
    }

    private SupportTicket(
        Guid id,
        string ticketNumber,
        Guid requesterUserId,
        SupportUnit targetUnit,
        string subject,
        TicketPriority priority,
        string initialMessage,
        string? attachmentUrl,
        DateTime createdAtUtc)
        : base(id)
    {
        if (requesterUserId == Guid.Empty)
        {
            throw new DomainException("SupportTicket.RequesterRequired");
        }

        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new DomainException("SupportTicket.SubjectRequired");
        }

        var trimmedSubject = subject.Trim();
        if (trimmedSubject.Length > SubjectMaxLength)
        {
            throw new DomainException("SupportTicket.SubjectTooLong");
        }

        TicketNumber = ticketNumber.Trim().ToUpperInvariant();
        RequesterUserId = requesterUserId;
        TargetUnit = targetUnit;
        Subject = trimmedSubject;
        Priority = priority;
        Status = TicketStatus.Open;
        CreatedAtUtc = createdAtUtc;

        _messages.Add(new TicketMessage(
            Guid.NewGuid(),
            id,
            requesterUserId,
            isFromStaff: false,
            initialMessage,
            attachmentUrl,
            createdAtUtc));
    }

    public static SupportTicket Create(
        Guid requesterUserId,
        SupportUnit targetUnit,
        string subject,
        TicketPriority priority,
        string initialMessage,
        string? attachmentUrl,
        DateTime createdAtUtc)
    {
        var ticketNumber = $"SRV-{createdAtUtc:yyyyMM}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

        return new SupportTicket(
            Guid.NewGuid(),
            ticketNumber,
            requesterUserId,
            targetUnit,
            subject,
            priority,
            initialMessage,
            attachmentUrl,
            createdAtUtc);
    }

    public void AddMessage(
        Guid senderUserId,
        bool isStaff,
        string content,
        string? attachmentUrl,
        DateTime sentAtUtc)
    {
        if (Status == TicketStatus.Closed)
        {
            throw new DomainException("SupportTicket.CannotReplyToClosedTicket");
        }

        _messages.Add(new TicketMessage(
            Guid.NewGuid(),
            Id,
            senderUserId,
            isStaff,
            content,
            attachmentUrl,
            sentAtUtc));

        Status = isStaff ? TicketStatus.Answered : TicketStatus.InProgress;
        UpdatedAtUtc = sentAtUtc;
    }

    public void AssignStaff(Guid staffUserId, DateTime updatedAtUtc)
    {
        if (staffUserId == Guid.Empty)
        {
            throw new DomainException("SupportTicket.StaffRequired");
        }

        AssignedStaffUserId = staffUserId;
        if (Status == TicketStatus.Open)
        {
            Status = TicketStatus.InProgress;
        }

        UpdatedAtUtc = updatedAtUtc;
    }

    public void Resolve(Guid staffUserId, string? resolutionMessage, DateTime resolvedAtUtc)
    {
        if (Status == TicketStatus.Closed)
        {
            throw new DomainException("SupportTicket.TicketAlreadyClosed");
        }

        if (!string.IsNullOrWhiteSpace(resolutionMessage))
        {
            AddMessage(staffUserId, isStaff: true, resolutionMessage, null, resolvedAtUtc);
        }

        Status = TicketStatus.Resolved;
        UpdatedAtUtc = resolvedAtUtc;
    }

    public void Close(DateTime closedAtUtc)
    {
        Status = TicketStatus.Closed;
        ClosedAtUtc = closedAtUtc;
        UpdatedAtUtc = closedAtUtc;
    }
}
