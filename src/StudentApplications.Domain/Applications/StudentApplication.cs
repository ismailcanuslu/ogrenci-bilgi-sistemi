using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace StudentApplications.Domain.Applications;

public sealed class StudentApplication : AggregateRoot<Guid>
{
    public const int ApplicationNumberMaxLength = 40;
    public const int TargetInstitutionMaxLength = 200;
    public const int DetailsMaxLength = 3000;
    public const int ReviewNotesMaxLength = 1000;

    public string ApplicationNumber { get; private set; } = null!;

    public Guid StudentId { get; private set; }

    public ApplicationType Type { get; private set; }

    public ApplicationState State { get; private set; }

    public Guid? TargetDepartmentId { get; private set; }

    public string? TargetInstitution { get; private set; }

    public string Details { get; private set; } = null!;

    public string? AttachmentUrl { get; private set; }

    public decimal? Gpa { get; private set; }

    public string? ReviewNotes { get; private set; }

    public Guid? ReviewerUserId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? ReviewedAtUtc { get; private set; }

    private StudentApplication()
    {
    }

    public StudentApplication(
        Guid id,
        Guid studentId,
        ApplicationType type,
        Guid? targetDepartmentId,
        string? targetInstitution,
        string details,
        string? attachmentUrl,
        decimal? gpa,
        DateTime createdAtUtc)
        : base(id)
    {
        if (studentId == Guid.Empty)
        {
            throw new DomainException("StudentApplication.StudentRequired");
        }

        if (string.IsNullOrWhiteSpace(details))
        {
            throw new DomainException("StudentApplication.DetailsRequired");
        }

        if ((type == ApplicationType.DoubleMajor || type == ApplicationType.Minor) && !targetDepartmentId.HasValue)
        {
            throw new DomainException("StudentApplication.TargetDepartmentRequired");
        }

        StudentId = studentId;
        Type = type;
        State = ApplicationState.Submitted;
        TargetDepartmentId = targetDepartmentId;
        TargetInstitution = targetInstitution?.Trim();
        Details = details.Trim();
        AttachmentUrl = attachmentUrl?.Trim();
        Gpa = gpa;
        CreatedAtUtc = createdAtUtc;
        ApplicationNumber = $"APP-{createdAtUtc:yyyyMM}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
    }

    public static StudentApplication Create(
        Guid studentId,
        ApplicationType type,
        Guid? targetDepartmentId,
        string? targetInstitution,
        string details,
        string? attachmentUrl,
        decimal? gpa,
        DateTime createdAtUtc)
    {
        return new StudentApplication(
            Guid.NewGuid(),
            studentId,
            type,
            targetDepartmentId,
            targetInstitution,
            details,
            attachmentUrl,
            gpa,
            createdAtUtc);
    }

    public void MarkUnderReview(Guid reviewerUserId, DateTime reviewedAtUtc)
    {
        if (State != ApplicationState.Submitted)
        {
            throw new DomainException("StudentApplication.NotSubmitted");
        }

        State = ApplicationState.UnderReview;
        ReviewerUserId = reviewerUserId;
        ReviewedAtUtc = reviewedAtUtc;
    }

    public void Approve(Guid reviewerUserId, string? notes, DateTime reviewedAtUtc)
    {
        if (State != ApplicationState.Submitted && State != ApplicationState.UnderReview)
        {
            throw new DomainException("StudentApplication.InvalidStateForReview");
        }

        State = ApplicationState.Approved;
        ReviewerUserId = reviewerUserId;
        ReviewNotes = notes?.Trim();
        ReviewedAtUtc = reviewedAtUtc;
    }

    public void Reject(Guid reviewerUserId, string notes, DateTime reviewedAtUtc)
    {
        if (State != ApplicationState.Submitted && State != ApplicationState.UnderReview)
        {
            throw new DomainException("StudentApplication.InvalidStateForReview");
        }

        if (string.IsNullOrWhiteSpace(notes))
        {
            throw new DomainException("StudentApplication.RejectionReasonRequired");
        }

        State = ApplicationState.Rejected;
        ReviewerUserId = reviewerUserId;
        ReviewNotes = notes.Trim();
        ReviewedAtUtc = reviewedAtUtc;
    }

    public void Cancel()
    {
        if (State != ApplicationState.Submitted)
        {
            throw new DomainException("StudentApplication.CannotCancelUnderReviewOrCompleted");
        }

        State = ApplicationState.Cancelled;
    }
}
