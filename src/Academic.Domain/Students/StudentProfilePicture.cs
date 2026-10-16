using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Students;

public enum ProfilePictureStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}

public sealed class StudentProfilePicture : Entity<Guid>
{
    public const int ImageUrlMaxLength = 1000;
    public const int RejectionReasonMaxLength = 500;

    public Guid StudentId { get; private set; }

    public string ImageUrl { get; private set; } = null!;

    public ProfilePictureStatus Status { get; private set; }

    public DateTime UploadedAtUtc { get; private set; }

    public DateTime? ReviewedAtUtc { get; private set; }

    public string? RejectionReason { get; private set; }

    private StudentProfilePicture()
    {
    }

    public StudentProfilePicture(
        Guid id,
        Guid studentId,
        string imageUrl,
        DateTime uploadedAtUtc)
        : base(id)
    {
        if (studentId == Guid.Empty)
        {
            throw new DomainException("StudentProfilePicture.StudentRequired");
        }

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            throw new DomainException("StudentProfilePicture.ImageUrlRequired");
        }

        StudentId = studentId;
        ImageUrl = imageUrl.Trim();
        Status = ProfilePictureStatus.Pending;
        UploadedAtUtc = uploadedAtUtc;
    }

    public void Approve(DateTime reviewedAtUtc)
    {
        Status = ProfilePictureStatus.Approved;
        ReviewedAtUtc = reviewedAtUtc;
        RejectionReason = null;
    }

    public void Reject(string reason, DateTime reviewedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainException("StudentProfilePicture.ReasonRequired");
        }

        Status = ProfilePictureStatus.Rejected;
        RejectionReason = reason.Trim();
        ReviewedAtUtc = reviewedAtUtc;
    }
}
