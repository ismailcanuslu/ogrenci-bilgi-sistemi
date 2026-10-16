using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Security.Domain.Authorization;

/// <summary>
/// Kullanıcılara atanabilen yetki grubu. İzinler (permission) tekil işlemleri tanımlar.
/// </summary>
public sealed class Role : AggregateRoot<Guid>
{
    public const int NameMaxLength = 100;

    private readonly List<RolePermission> _permissions = [];

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    /// <summary>Sistem tarafından seed edilen roller silinemez/yeniden adlandırılamaz.</summary>
    public bool IsSystem { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    private Role()
    {
    }

    private Role(Guid id, string name, string? description, bool isSystem, DateTime createdAtUtc)
        : base(id)
    {
        Name = name;
        Description = description;
        IsSystem = isSystem;
        CreatedAtUtc = createdAtUtc;
    }

    public static Role Create(
        string name,
        string? description,
        IEnumerable<string> permissions,
        DateTime createdAtUtc,
        bool isSystem = false)
    {
        var normalizedName = NormalizeName(name);
        ArgumentNullException.ThrowIfNull(permissions);

        var role = new Role(Guid.NewGuid(), normalizedName, description?.Trim(), isSystem, createdAtUtc);
        role.SetPermissions(permissions);
        return role;
    }

    public void SetPermissions(IEnumerable<string> permissions)
    {
        ArgumentNullException.ThrowIfNull(permissions);

        var normalized = permissions
            .Select(p => p?.Trim().ToLowerInvariant())
            .Where(p => !string.IsNullOrEmpty(p))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        _permissions.RemoveAll(existing => !normalized.Contains(existing.Permission));

        foreach (var permission in normalized)
        {
            if (_permissions.All(existing => existing.Permission != permission))
            {
                _permissions.Add(new RolePermission(Id, permission!));
            }
        }
    }

    public bool HasPermission(string permission)
        => _permissions.Any(p => p.Permission == permission);

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Security.Role.NameRequired");
        }

        var trimmed = name.Trim();

        if (trimmed.Length > NameMaxLength)
        {
            throw new DomainException("Security.Role.NameTooLong");
        }

        return trimmed;
    }
}
