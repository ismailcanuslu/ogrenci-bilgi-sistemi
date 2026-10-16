using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Courses;

public sealed class Course : AggregateRoot<Guid>
{
    public const int CodeMaxLength = 30;
    public const int NameMaxLength = 200;
    public const int DescriptionMaxLength = 2000;
    public const int PrerequisitesMaxLength = 500;

    public Guid DepartmentId { get; private set; }

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public int DefaultEcts { get; private set; }

    public int DefaultCredit { get; private set; }

    public string? Prerequisites { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    private Course()
    {
    }

    private Course(
        Guid id,
        Guid departmentId,
        string code,
        string name,
        string? description,
        int defaultEcts,
        int defaultCredit,
        string? prerequisites,
        DateTime createdAtUtc)
        : base(id)
    {
        DepartmentId = departmentId;
        Code = NormalizeCode(code);
        Name = NormalizeName(name);
        Description = description?.Trim();
        DefaultEcts = ValidateEcts(defaultEcts);
        DefaultCredit = ValidateCredit(defaultCredit);
        Prerequisites = NormalizePrerequisites(prerequisites);
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public static Course Create(
        Guid departmentId,
        string code,
        string name,
        string? description,
        int defaultEcts,
        int defaultCredit,
        string? prerequisites,
        DateTime createdAtUtc)
    {
        if (departmentId == Guid.Empty)
        {
            throw new DomainException("Course.DepartmentRequired");
        }

        return new Course(
            Guid.NewGuid(),
            departmentId,
            code,
            name,
            description,
            defaultEcts,
            defaultCredit,
            prerequisites,
            createdAtUtc);
    }

    public void Update(
        string name,
        string? description,
        int defaultEcts,
        int defaultCredit,
        string? prerequisites,
        DateTime updatedAtUtc)
    {
        EnsureActive();

        Name = NormalizeName(name);
        Description = description?.Trim();
        DefaultEcts = ValidateEcts(defaultEcts);
        DefaultCredit = ValidateCredit(defaultCredit);
        Prerequisites = NormalizePrerequisites(prerequisites);
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Deactivate(DateTime updatedAtUtc)
    {
        if (!IsActive)
        {
            throw new DomainException("Course.AlreadyInactive");
        }

        IsActive = false;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Activate(DateTime updatedAtUtc)
    {
        if (IsActive)
        {
            throw new DomainException("Course.AlreadyActive");
        }

        IsActive = true;
        UpdatedAtUtc = updatedAtUtc;
    }

    private void EnsureActive()
    {
        if (!IsActive)
        {
            throw new DomainException("Course.Inactive");
        }
    }

    public static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Course.CodeRequired");
        }

        var trimmed = code.Trim().ToUpperInvariant();
        if (trimmed.Length > CodeMaxLength)
        {
            throw new DomainException("Course.CodeTooLong");
        }

        return trimmed;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Course.NameRequired");
        }

        var trimmed = name.Trim();
        if (trimmed.Length > NameMaxLength)
        {
            throw new DomainException("Course.NameTooLong");
        }

        return trimmed;
    }

    private static int ValidateEcts(int ects)
    {
        if (ects < 1 || ects > 60)
        {
            throw new DomainException("Course.InvalidEcts");
        }

        return ects;
    }

    private static int ValidateCredit(int credit)
    {
        if (credit < 0 || credit > 30)
        {
            throw new DomainException("Course.InvalidCredit");
        }

        return credit;
    }

    private static string? NormalizePrerequisites(string? prerequisites)
    {
        if (string.IsNullOrWhiteSpace(prerequisites))
        {
            return null;
        }

        var items = prerequisites
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(p => p.ToUpperInvariant())
            .Distinct();

        var result = string.Join(",", items);
        if (result.Length > PrerequisitesMaxLength)
        {
            throw new DomainException("Course.PrerequisitesTooLong");
        }

        return result;
    }
}
