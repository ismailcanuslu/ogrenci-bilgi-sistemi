using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Courses;

public sealed class CourseOfferingInstructor : Entity<Guid>
{
    public const int RoleOrTitleMaxLength = 100;

    public Guid CourseOfferingId { get; private set; }

    public Guid InstructorId { get; private set; }

    public bool IsPrimary { get; private set; }

    public string? RoleOrTitle { get; private set; }

    public DateTime AssignedAtUtc { get; private set; }

    private CourseOfferingInstructor()
    {
    }

    internal CourseOfferingInstructor(
        Guid id,
        Guid courseOfferingId,
        Guid instructorId,
        bool isPrimary,
        string? roleOrTitle,
        DateTime assignedAtUtc)
        : base(id)
    {
        if (instructorId == Guid.Empty)
        {
            throw new DomainException("CourseOfferingInstructor.InstructorRequired");
        }

        var trimmedRole = roleOrTitle?.Trim();
        if (trimmedRole?.Length > RoleOrTitleMaxLength)
        {
            throw new DomainException("CourseOfferingInstructor.RoleOrTitleTooLong");
        }

        CourseOfferingId = courseOfferingId;
        InstructorId = instructorId;
        IsPrimary = isPrimary;
        RoleOrTitle = trimmedRole;
        AssignedAtUtc = assignedAtUtc;
    }

    internal void SetPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }

    internal void UpdateRole(string? roleOrTitle)
    {
        var trimmed = roleOrTitle?.Trim();
        if (trimmed?.Length > RoleOrTitleMaxLength)
        {
            throw new DomainException("CourseOfferingInstructor.RoleOrTitleTooLong");
        }

        RoleOrTitle = trimmed;
    }
}
