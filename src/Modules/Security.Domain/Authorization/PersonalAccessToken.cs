using BuildingBlocks.Domain.Authorization;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Security.Domain.Authorization;

public sealed class PersonalAccessTokenPermission : Entity<Guid>
{
    public Guid PersonalAccessTokenId { get; private set; }

    public string Permission { get; private set; } = null!;

    private PersonalAccessTokenPermission()
    {
    }

    internal PersonalAccessTokenPermission(Guid id, Guid personalAccessTokenId, string permission)
        : base(id)
    {
        PersonalAccessTokenId = personalAccessTokenId;
        Permission = permission.Trim().ToLowerInvariant();
    }
}

public sealed class PersonalAccessToken : AggregateRoot<Guid>
{
    public const int NameMaxLength = 200;
    public const int TokenHashMaxLength = 128;
    public const int MaskedTokenMaxLength = 64;

    private readonly List<PersonalAccessTokenPermission> _permissions = [];

    public string Name { get; private set; } = null!;

    public string TokenHash { get; private set; } = null!;

    public string MaskedToken { get; private set; } = null!;

    public Guid CreatedByUserId { get; private set; }

    public ScopeType ScopeType { get; private set; }

    public Guid? ScopeId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? ExpiresAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    public Guid? RevokedByUserId { get; private set; }

    public DateTime? LastUsedAtUtc { get; private set; }

    public IReadOnlyCollection<PersonalAccessTokenPermission> Permissions => _permissions.AsReadOnly();

    private PersonalAccessToken()
    {
    }

    private PersonalAccessToken(
        Guid id,
        string name,
        string tokenHash,
        string maskedToken,
        Guid createdByUserId,
        ScopeType scopeType,
        Guid? scopeId,
        DateTime? expiresAtUtc,
        DateTime createdAtUtc)
        : base(id)
    {
        Name = NormalizeName(name);
        TokenHash = tokenHash;
        MaskedToken = maskedToken;
        CreatedByUserId = createdByUserId;
        ScopeType = scopeType;
        ScopeId = scopeId;
        ExpiresAtUtc = expiresAtUtc;
        CreatedAtUtc = createdAtUtc;
    }

    public static PersonalAccessToken Create(
        string name,
        string tokenHash,
        string maskedToken,
        IEnumerable<string> permissions,
        ScopeType scopeType,
        Guid? scopeId,
        Guid createdByUserId,
        DateTime? expiresAtUtc,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new DomainException("Pat.TokenHashRequired");
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new DomainException("Pat.CreatorRequired");
        }

        if (expiresAtUtc.HasValue && expiresAtUtc.Value <= createdAtUtc)
        {
            throw new DomainException("Pat.ExpirationMustBeInFuture");
        }

        if (scopeType != ScopeType.University && scopeId is null)
        {
            throw new DomainException("Pat.ScopeIdRequiredForNarrowScope");
        }

        var pat = new PersonalAccessToken(
            Guid.NewGuid(),
            name,
            tokenHash,
            maskedToken,
            createdByUserId,
            scopeType,
            scopeId,
            expiresAtUtc,
            createdAtUtc);

        var distinctPerms = permissions
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim().ToLowerInvariant())
            .Distinct();

        foreach (var perm in distinctPerms)
        {
            pat._permissions.Add(new PersonalAccessTokenPermission(Guid.NewGuid(), pat.Id, perm));
        }

        if (pat._permissions.Count == 0)
        {
            throw new DomainException("Pat.AtLeastOnePermissionRequired");
        }

        return pat;
    }

    public bool IsActive(DateTime nowUtc)
    {
        return RevokedAtUtc == null && (ExpiresAtUtc == null || ExpiresAtUtc > nowUtc);
    }

    public void Revoke(Guid revokedByUserId, DateTime revokedAtUtc)
    {
        if (RevokedAtUtc != null)
        {
            throw new DomainException("Pat.AlreadyRevoked");
        }

        RevokedAtUtc = revokedAtUtc;
        RevokedByUserId = revokedByUserId;
    }

    public void RecordUsage(DateTime usedAtUtc)
    {
        LastUsedAtUtc = usedAtUtc;
    }

    public bool HasPermission(string permission)
    {
        var normalized = permission.Trim().ToLowerInvariant();
        return _permissions.Any(p => p.Permission == normalized);
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Pat.NameRequired");
        }

        var trimmed = name.Trim();
        if (trimmed.Length > NameMaxLength)
        {
            throw new DomainException("Pat.NameTooLong");
        }

        return trimmed;
    }
}
