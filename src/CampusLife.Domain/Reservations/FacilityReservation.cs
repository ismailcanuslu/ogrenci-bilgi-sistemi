using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace CampusLife.Domain.Reservations;

public sealed class FacilityReservation : AggregateRoot<Guid>
{
    public const int FacilityNameMaxLength = 150;
    public const int NotesMaxLength = 500;
    public const int ReviewNotesMaxLength = 500;

    public Guid UserId { get; private set; }

    public FacilityType FacilityType { get; private set; }

    public string FacilityName { get; private set; } = null!;

    public DateOnly ReservationDate { get; private set; }

    public TimeSpan StartTime { get; private set; }

    public TimeSpan EndTime { get; private set; }

    public ReservationStatus Status { get; private set; }

    public string? Notes { get; private set; }

    public Guid? ReviewedByUserId { get; private set; }

    public string? ReviewNotes { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? ReviewedAtUtc { get; private set; }

    private FacilityReservation()
    {
    }

    private FacilityReservation(
        Guid id,
        Guid userId,
        FacilityType facilityType,
        string facilityName,
        DateOnly reservationDate,
        TimeSpan startTime,
        TimeSpan endTime,
        string? notes,
        DateTime createdAtUtc)
        : base(id)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException("FacilityReservation.UserRequired");
        }

        if (string.IsNullOrWhiteSpace(facilityName))
        {
            throw new DomainException("FacilityReservation.FacilityNameRequired");
        }

        if (startTime >= endTime)
        {
            throw new DomainException("FacilityReservation.InvalidTimeRange");
        }

        UserId = userId;
        FacilityType = facilityType;
        FacilityName = facilityName.Trim();
        ReservationDate = reservationDate;
        StartTime = startTime;
        EndTime = endTime;
        Status = ReservationStatus.Pending;
        Notes = notes?.Trim();
        CreatedAtUtc = createdAtUtc;
    }

    public static FacilityReservation Create(
        Guid userId,
        FacilityType facilityType,
        string facilityName,
        DateOnly reservationDate,
        TimeSpan startTime,
        TimeSpan endTime,
        string? notes,
        DateTime createdAtUtc)
    {
        return new FacilityReservation(
            Guid.NewGuid(),
            userId,
            facilityType,
            facilityName,
            reservationDate,
            startTime,
            endTime,
            notes,
            createdAtUtc);
    }

    public void Approve(Guid reviewerUserId, string? reviewNotes, DateTime reviewedAtUtc)
    {
        EnsurePending();

        if (reviewerUserId == Guid.Empty)
        {
            throw new DomainException("FacilityReservation.ReviewerRequired");
        }

        Status = ReservationStatus.Approved;
        ReviewedByUserId = reviewerUserId;
        ReviewNotes = reviewNotes?.Trim();
        ReviewedAtUtc = reviewedAtUtc;
    }

    public void Reject(Guid reviewerUserId, string reviewNotes, DateTime reviewedAtUtc)
    {
        EnsurePending();

        if (reviewerUserId == Guid.Empty)
        {
            throw new DomainException("FacilityReservation.ReviewerRequired");
        }

        if (string.IsNullOrWhiteSpace(reviewNotes))
        {
            throw new DomainException("FacilityReservation.RejectionReasonRequired");
        }

        Status = ReservationStatus.Rejected;
        ReviewedByUserId = reviewerUserId;
        ReviewNotes = reviewNotes.Trim();
        ReviewedAtUtc = reviewedAtUtc;
    }

    public void Cancel(Guid cancellingUserId, string? cancellationReason, DateTime cancelledAtUtc)
    {
        if (Status == ReservationStatus.Completed || Status == ReservationStatus.Cancelled)
        {
            throw new DomainException("FacilityReservation.CannotCancelFinalizedReservation");
        }

        Status = ReservationStatus.Cancelled;
        ReviewNotes = cancellationReason?.Trim();
        ReviewedAtUtc = cancelledAtUtc;
    }

    public void Complete(DateTime completedAtUtc)
    {
        if (Status != ReservationStatus.Approved)
        {
            throw new DomainException("FacilityReservation.OnlyApprovedReservationsCanBeCompleted");
        }

        Status = ReservationStatus.Completed;
        ReviewedAtUtc = completedAtUtc;
    }

    public bool OverlapsWith(DateOnly date, TimeSpan start, TimeSpan end)
    {
        if (ReservationDate != date)
        {
            return false;
        }

        if (Status == ReservationStatus.Rejected || Status == ReservationStatus.Cancelled)
        {
            return false;
        }

        return StartTime < end && start < EndTime;
    }

    private void EnsurePending()
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new DomainException("FacilityReservation.NotPending");
        }
    }
}
