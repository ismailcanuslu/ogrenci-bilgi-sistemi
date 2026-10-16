using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;
using Security.Domain.Errors;

namespace Security.Domain.Entities;

public sealed class Session : AggregateRoot<Guid>
{
    private readonly List<RefreshToken> _refreshTokens = [];

    public Guid UserId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime? LastActivityAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    public IReadOnlyCollection<RefreshToken> RefreshTokens =>
        _refreshTokens.AsReadOnly();

    public bool IsRevoked =>
        RevokedAtUtc is not null;

    private Session()
    {
    }

    private Session(
        Guid id,
        Guid userId,
        DateTime createdAtUtc,
        DateTime expiresAtUtc)
        : base(id)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException(
                SecurityErrors.Session.UserIdRequired);
        }

        if (expiresAtUtc <= createdAtUtc)
        {
            throw new DomainException(
                SecurityErrors.Session.ExpiryBeforeCreation);
        }

        UserId = userId;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }

    public static Session Create(
        Guid userId,
        DateTime createdAtUtc,
        DateTime expiresAtUtc)
    {
        return new Session(
            Guid.NewGuid(),
            userId,
            createdAtUtc,
            expiresAtUtc);
    }

    public RefreshToken AddRefreshToken(
        byte[] tokenHash,
        DateTime createdAtUtc,
        DateTime expiresAtUtc)
    {
        EnsureActive(createdAtUtc);

        var refreshToken = RefreshToken.Create(
            Id,
            tokenHash,
            createdAtUtc,
            expiresAtUtc);

        _refreshTokens.Add(refreshToken);

        LastActivityAtUtc = createdAtUtc;

        return refreshToken;
    }

    public RefreshToken RotateRefreshToken(
        Guid refreshTokenId,
        byte[] currentTokenHash,
        byte[] replacementTokenHash,
        DateTime rotatedAtUtc,
        DateTime replacementExpiresAtUtc)
    {
        EnsureActive(rotatedAtUtc);

        var currentToken = _refreshTokens
            .FirstOrDefault(x => x.Id == refreshTokenId);

        if (currentToken is null)
        {
            throw new DomainException(
                SecurityErrors.Session.RefreshTokenNotFound);
        }

        var replacementToken = RefreshToken.Create(
            Id,
            replacementTokenHash,
            rotatedAtUtc,
            replacementExpiresAtUtc);

        currentToken.Use(
            currentTokenHash,
            rotatedAtUtc,
            replacementToken.Id);

        _refreshTokens.Add(replacementToken);

        LastActivityAtUtc = rotatedAtUtc;

        return replacementToken;
    }

    public void Revoke(DateTime revokedAtUtc)
    {
        if (IsRevoked)
        {
            return;
        }

        if (revokedAtUtc < CreatedAtUtc)
        {
            throw new DomainException(
                SecurityErrors.Session.RevokedBeforeCreation);
        }

        RevokedAtUtc = revokedAtUtc;

        foreach (var refreshToken in _refreshTokens)
        {
            refreshToken.Revoke(revokedAtUtc);
        }
    }

    private void EnsureActive(DateTime nowUtc)
    {
        if (IsRevoked)
        {
            throw new DomainException(
                SecurityErrors.Session.Revoked);
        }

        if (nowUtc >= ExpiresAtUtc)
        {
            throw new DomainException(
                SecurityErrors.Session.Expired);
        }
    }
}