using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Programs;

public sealed class Program : AggregateRoot<Guid>
{
    public const int CodeMaxLength = 30;
    public const int NameMaxLength = 200;

    public Guid DepartmentId { get; private set; }

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public DegreeLevel DegreeLevel { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    private Program()
    {
    }

    private Program(
        Guid id,
        Guid departmentId,
        string code,
        string name,
        DegreeLevel degreeLevel,
        DateTime createdAtUtc)
        : base(id)
    {
        DepartmentId = departmentId;
        Code = NormalizeCode(code);
        Name = NormalizeName(name);
        DegreeLevel = degreeLevel;
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public static Program Create(
        Guid departmentId,
        string code,
        string name,
        DegreeLevel degreeLevel,
        DateTime createdAtUtc)
    {
        if (departmentId == Guid.Empty)
        {
            throw new DomainException("Program.DepartmentRequired");
        }

        if (!Enum.IsDefined(degreeLevel))
        {
            throw new DomainException("Program.InvalidDegreeLevel");
        }

        return new Program(Guid.NewGuid(), departmentId, code, name, degreeLevel, createdAtUtc);
    }

    public void Update(string code, string name, DegreeLevel degreeLevel, DateTime updatedAtUtc)
    {
        EnsureActive();

        if (!Enum.IsDefined(degreeLevel))
        {
            throw new DomainException("Program.InvalidDegreeLevel");
        }

        Code = NormalizeCode(code);
        Name = NormalizeName(name);
        DegreeLevel = degreeLevel;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Deactivate(DateTime updatedAtUtc)
    {
        if (!IsActive)
        {
            throw new DomainException("Program.AlreadyInactive");
        }

        IsActive = false;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Activate(DateTime updatedAtUtc)
    {
        if (IsActive)
        {
            throw new DomainException("Program.AlreadyActive");
        }

        IsActive = true;
        UpdatedAtUtc = updatedAtUtc;
    }

    private void EnsureActive()
    {
        if (!IsActive)
        {
            throw new DomainException("Program.Inactive");
        }
    }

    public static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Program.CodeRequired");
        }

        var trimmed = code.Trim().ToUpperInvariant();
        if (trimmed.Length > CodeMaxLength)
        {
            throw new DomainException("Program.CodeTooLong");
        }

        return trimmed;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Program.NameRequired");
        }

        var trimmed = name.Trim();
        if (trimmed.Length > NameMaxLength)
        {
            throw new DomainException("Program.NameTooLong");
        }

        return trimmed;
    }
}
