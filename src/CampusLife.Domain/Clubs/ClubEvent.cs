using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace CampusLife.Domain.Clubs;

public sealed class ClubEvent : AggregateRoot<Guid>
{
    public const int TitleMaxLength = 200;
    public const int LocationMaxLength = 150;
    public const int DescriptionMaxLength = 2000;
    public const int ReviewNotesMaxLength = 500;

    public Guid ClubId { get; private set; }

    public string Title { get; private set; } = null!;

    public string? Description { get; private set; }

    public DateTime EventDateUtc { get; private set; }

    public string Location { get; private set; } = null!;

    public int Capacity { get; private set; }

    public EventStatus Status { get; private set; }

    public Guid? ReviewedByUserId { get; private set; }

    public string? ReviewNotes { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? ReviewedAtUtc { get; private set; }

    private ClubEvent()
    {
    }

    public ClubEvent(
        Guid id,
        Guid clubId,
        string title,
        string? description,
        DateTime eventDateUtc,
        string location,
        int capacity,
        DateTime createdAtUtc)
        : base(id)
    {
        if (clubId == Guid.Empty)
        {
            throw new DomainException("ClubEvent.ClubRequired");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("ClubEvent.TitleRequired");
        }

        if (string.IsNullOrWhiteSpace(location))
        {
            throw new DomainException("ClubEvent.LocationRequired");
        }

        if (capacity < 1)
        {
            throw new DomainException("ClubEvent.InvalidCapacity");
        }

        ClubId = clubId;
        Title = title.Trim();
        Description = description?.Trim();
        EventDateUtc = eventDateUtc;
        Location = location.Trim();
        Capacity = capacity;
        Status = EventStatus.PendingApproval;
        CreatedAtUtc = createdAtUtc;
    }

    public static ClubEvent Create(
        Guid clubId,
        string title,
        string? description,
        DateTime eventDateUtc,
        string location,
        int capacity,
        DateTime createdAtUtc)
    {
        return new ClubEvent(
            Guid.NewGuid(),
            clubId,
            title,
            description,
            eventDateUtc,
            location,
            capacity,
            createdAtUtc);
    }

    public void Approve(Guid reviewerUserId, string? reviewNotes, DateTime reviewedAtUtc)
    {
        if (Status != EventStatus.PendingApproval)
        {
            throw new DomainException("ClubEvent.NotPendingApproval");
        }

        Status = EventStatus.Approved;
        ReviewedByUserId = reviewerUserId;
        ReviewNotes = reviewNotes?.Trim();
        ReviewedAtUtc = reviewedAtUtc;
    }

    public void Reject(Guid reviewerUserId, string reviewNotes, DateTime reviewedAtUtc)
    {
        if (Status != EventStatus.PendingApproval)
        {
            throw new DomainException("ClubEvent.NotPendingApproval");
        }

        if (string.IsNullOrWhiteSpace(reviewNotes))
        {
            throw new DomainException("ClubEvent.RejectionReasonRequired");
        }

        Status = EventStatus.Rejected;
        ReviewedByUserId = reviewerUserId;
        ReviewNotes = reviewNotes.Trim();
        ReviewedAtUtc = reviewedAtUtc;
    }
}
