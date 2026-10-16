using BuildingBlocks.Domain.Primitives;

namespace SocialContribution.Domain;

public sealed class ProjectVolunteer : Entity<Guid>
{
    public Guid ProjectId { get; private set; }
    public Guid UserId { get; private set; }
    public string StudentOrStaffNo { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;
    public VolunteerApplicationStatus Status { get; private set; }
    public DateTime AppliedAtUtc { get; private set; }
    public DateTime? EvaluatedAtUtc { get; private set; }

    private ProjectVolunteer() { }

    internal static ProjectVolunteer Create(
        Guid id,
        Guid projectId,
        Guid userId,
        string studentOrStaffNo,
        string fullName,
        string email,
        string note)
    {
        return new ProjectVolunteer
        {
            Id = id,
            ProjectId = projectId,
            UserId = userId,
            StudentOrStaffNo = studentOrStaffNo.Trim(),
            FullName = fullName.Trim(),
            Email = email.Trim(),
            Note = note.Trim(),
            Status = VolunteerApplicationStatus.Pending,
            AppliedAtUtc = DateTime.UtcNow
        };
    }

    internal void SetStatus(VolunteerApplicationStatus status)
    {
        Status = status;
        EvaluatedAtUtc = DateTime.UtcNow;
    }
}
