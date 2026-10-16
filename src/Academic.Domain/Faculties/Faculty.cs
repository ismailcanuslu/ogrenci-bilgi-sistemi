using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Faculties;

public sealed class Faculty : AggregateRoot<Guid>
{
    public const int CodeMaxLength = 20;
    public const int NameMaxLength = 200;

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    /// <summary>Kullanımdaki kayıtlar silinmez; geçmiş akademik veriyi korumak için pasifleştirilir.</summary>
    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    private Faculty()
    {
    }

    private Faculty(Guid id, string code, string name, DateTime createdAtUtc)
        : base(id)
    {
        Code = code;
        Name = name;
        IsActive = true;
        CreatedAtUtc = createdAtUtc;
    }

    public static Faculty Create(string code, string name, DateTime createdAtUtc)
        => new(Guid.NewGuid(), NormalizeCode(code), NormalizeName(name), createdAtUtc);

    public void Update(string code, string name, DateTime updatedAtUtc)
    {
        EnsureActive();

        Code = NormalizeCode(code);
        Name = NormalizeName(name);
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Deactivate(DateTime updatedAtUtc)
    {
        if (!IsActive)
        {
            throw new DomainException("Faculty.AlreadyInactive");
        }

        IsActive = false;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Activate(DateTime updatedAtUtc)
    {
        if (IsActive)
        {
            throw new DomainException("Faculty.AlreadyActive");
        }

        IsActive = true;
        UpdatedAtUtc = updatedAtUtc;
    }

    private void EnsureActive()
    {
        if (!IsActive)
        {
            throw new DomainException("Faculty.Inactive");
        }
    }

    public static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Faculty.CodeRequired");
        }

        var normalized = code.Trim().ToUpperInvariant();

        if (normalized.Length > CodeMaxLength)
        {
            throw new DomainException("Faculty.CodeTooLong");
        }

        return normalized;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Faculty.NameRequired");
        }

        var normalized = name.Trim();

        if (normalized.Length > NameMaxLength)
        {
            throw new DomainException("Faculty.NameTooLong");
        }

        return normalized;
    }
}
