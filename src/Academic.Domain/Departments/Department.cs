using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Departments;

public sealed class Department : AggregateRoot<Guid>
{
    public const int CodeMaxLength = 20;
    public const int NameMaxLength = 200;

    public Guid FacultyId { get; private set; }

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    private Department()
    {
    }

    private Department(Guid id, Guid facultyId, string code, string name, DateTime createdAtUtc)
        : base(id)
    {
        FacultyId = facultyId;
        Code = code;
        Name = name;
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public static Department Create(Guid facultyId, string code, string name, DateTime createdAtUtc)
    {
        if (facultyId == Guid.Empty)
        {
            throw new DomainException("Department.FacultyRequired");
        }

        return new Department(Guid.NewGuid(), facultyId, NormalizeCode(code), NormalizeName(name), createdAtUtc);
    }

    /// <summary>
    /// Bölümün bağlı olduğu fakülte değiştirilemez; geçmiş akademik kayıtlar fakülteye bağlıdır.
    /// </summary>
    public void Update(string code, string name, DateTime updatedAtUtc)
    {
        if (!IsActive)
        {
            throw new DomainException("Department.Inactive");
        }

        Code = NormalizeCode(code);
        Name = NormalizeName(name);
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Deactivate(DateTime updatedAtUtc)
    {
        if (!IsActive)
        {
            throw new DomainException("Department.AlreadyInactive");
        }

        IsActive = false;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Activate(DateTime updatedAtUtc)
    {
        if (IsActive)
        {
            throw new DomainException("Department.AlreadyActive");
        }

        IsActive = true;
        UpdatedAtUtc = updatedAtUtc;
    }

    public static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Department.CodeRequired");
        }

        var normalized = code.Trim().ToUpperInvariant();

        if (normalized.Length > CodeMaxLength)
        {
            throw new DomainException("Department.CodeTooLong");
        }

        return normalized;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Department.NameRequired");
        }

        var normalized = name.Trim();

        if (normalized.Length > NameMaxLength)
        {
            throw new DomainException("Department.NameTooLong");
        }

        return normalized;
    }
}
