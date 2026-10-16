using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Instructors;

public sealed class InstructorDepartmentAffiliation : AggregateRoot<Guid>
{
    public const int TitleMaxLength = 100;

    public Guid InstructorId { get; private set; }

    public Guid FacultyId { get; private set; }

    public Guid DepartmentId { get; private set; }

    public AffiliationType AffiliationType { get; private set; }

    public string? Title { get; private set; }

    public bool IsPrimary { get; private set; }

    public DateTime StartDateUtc { get; private set; }

    public DateTime? EndDateUtc { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    private InstructorDepartmentAffiliation()
    {
    }

    public InstructorDepartmentAffiliation(
        Guid id,
        Guid instructorId,
        Guid facultyId,
        Guid departmentId,
        AffiliationType affiliationType,
        string? title,
        bool isPrimary,
        DateTime startDateUtc,
        DateTime? endDateUtc,
        DateTime createdAtUtc)
        : base(id)
    {
        if (instructorId == Guid.Empty)
        {
            throw new DomainException("InstructorAffiliation.InstructorRequired");
        }

        if (facultyId == Guid.Empty)
        {
            throw new DomainException("InstructorAffiliation.FacultyRequired");
        }

        if (departmentId == Guid.Empty)
        {
            throw new DomainException("InstructorAffiliation.DepartmentRequired");
        }

        InstructorId = instructorId;
        FacultyId = facultyId;
        DepartmentId = departmentId;
        AffiliationType = affiliationType;
        Title = ValidateTitle(title);
        IsPrimary = isPrimary;
        StartDateUtc = startDateUtc;
        EndDateUtc = endDateUtc;
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public static InstructorDepartmentAffiliation Create(
        Guid instructorId,
        Guid facultyId,
        Guid departmentId,
        AffiliationType affiliationType,
        string? title,
        bool isPrimary,
        DateTime startDateUtc,
        DateTime? endDateUtc,
        DateTime createdAtUtc)
    {
        return new InstructorDepartmentAffiliation(
            Guid.NewGuid(),
            instructorId,
            facultyId,
            departmentId,
            affiliationType,
            title,
            isPrimary,
            startDateUtc,
            endDateUtc,
            createdAtUtc);
    }

    public void SetPrimary(bool isPrimary, DateTime updatedAtUtc)
    {
        IsPrimary = isPrimary;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void UpdateAffiliation(
        AffiliationType affiliationType,
        string? title,
        DateTime? endDateUtc,
        DateTime updatedAtUtc)
    {
        AffiliationType = affiliationType;
        Title = ValidateTitle(title);
        EndDateUtc = endDateUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void SetActive(bool isActive, DateTime updatedAtUtc)
    {
        IsActive = isActive;
        if (!isActive && EndDateUtc is null)
        {
            EndDateUtc = updatedAtUtc;
        }

        UpdatedAtUtc = updatedAtUtc;
    }

    private static string? ValidateTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return null;
        }

        var trimmed = title.Trim();
        if (trimmed.Length > TitleMaxLength)
        {
            throw new DomainException("InstructorAffiliation.TitleTooLong");
        }

        return trimmed;
    }
}
