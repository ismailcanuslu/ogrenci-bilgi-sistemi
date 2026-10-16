using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace ContinuingEducation.Domain.Programs;

public sealed class ProgramApplication : AggregateRoot<Guid>
{
    public const int CertificateNumberMaxLength = 40;

    public Guid ProgramId { get; private set; }

    public Guid ApplicantUserId { get; private set; }

    public ApplicationStatus Status { get; private set; }

    public int AttendedHours { get; private set; }

    public decimal? FinalGrade { get; private set; }

    public bool IsCertificateEarned { get; private set; }

    public string? CertificateNumber { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? ReviewedAtUtc { get; private set; }

    private ProgramApplication()
    {
    }

    public ProgramApplication(
        Guid id,
        Guid programId,
        Guid applicantUserId,
        DateTime createdAtUtc)
        : base(id)
    {
        if (programId == Guid.Empty)
        {
            throw new DomainException("ProgramApplication.ProgramRequired");
        }

        if (applicantUserId == Guid.Empty)
        {
            throw new DomainException("ProgramApplication.ApplicantRequired");
        }

        ProgramId = programId;
        ApplicantUserId = applicantUserId;
        Status = ApplicationStatus.Pending;
        AttendedHours = 0;
        IsCertificateEarned = false;
        CreatedAtUtc = createdAtUtc;
    }

    public static ProgramApplication Create(
        Guid programId,
        Guid applicantUserId,
        DateTime createdAtUtc)
    {
        return new ProgramApplication(
            Guid.NewGuid(),
            programId,
            applicantUserId,
            createdAtUtc);
    }

    public void Approve(DateTime approvedAtUtc)
    {
        if (Status != ApplicationStatus.Pending)
        {
            throw new DomainException("ProgramApplication.NotPending");
        }

        Status = ApplicationStatus.Approved;
        ReviewedAtUtc = approvedAtUtc;
    }

    public void Reject(string? reason, DateTime rejectedAtUtc)
    {
        if (Status != ApplicationStatus.Pending)
        {
            throw new DomainException("ProgramApplication.NotPending");
        }

        Status = ApplicationStatus.Rejected;
        ReviewedAtUtc = rejectedAtUtc;
    }

    public void AddAttendedHours(int hours)
    {
        if (Status != ApplicationStatus.Approved)
        {
            throw new DomainException("ProgramApplication.NotApproved");
        }

        if (hours < 0)
        {
            throw new DomainException("ProgramApplication.InvalidHours");
        }

        AttendedHours += hours;
    }

    public void FinalizeEvaluation(
        decimal finalGrade,
        int totalProgramHours,
        int minAttendancePercentage,
        decimal minPassingGrade,
        DateTime evaluatedAtUtc)
    {
        if (Status != ApplicationStatus.Approved)
        {
            throw new DomainException("ProgramApplication.NotApproved");
        }

        FinalGrade = finalGrade;
        var attendancePercentage = totalProgramHours > 0
            ? (int)Math.Round((double)AttendedHours / totalProgramHours * 100)
            : 100;

        // Hem devamsızlık sınırı hem de geçme notu kontrolü
        if (attendancePercentage >= minAttendancePercentage && finalGrade >= minPassingGrade)
        {
            Status = ApplicationStatus.Completed;
            IsCertificateEarned = true;
            CertificateNumber = $"SEM-{evaluatedAtUtc:yyyy}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        }
        else
        {
            Status = ApplicationStatus.Failed;
            IsCertificateEarned = false;
        }

        ReviewedAtUtc = evaluatedAtUtc;
    }
}
