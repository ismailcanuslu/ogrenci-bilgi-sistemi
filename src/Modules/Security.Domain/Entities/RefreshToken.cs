using System.Security.Cryptography;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;
using Security.Domain.Errors;

namespace Security.Domain.Entities;

public sealed class RefreshToken : Entity<Guid>
{
    public Guid SessionId { get; private set; }

    public byte[] TokenHash { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime? UsedAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    public Guid? ReplacedByTokenId { get; private set; }

    public bool IsUsed => UsedAtUtc is not null;

    public bool IsRevoked => RevokedAtUtc is not null;

    private RefreshToken()
    {
    }

    private RefreshToken(
        Guid id,
        Guid sessionId,
        byte[] tokenHash,
        DateTime createdAtUtc,
        DateTime expiresAtUtc)
        : base(id)
    {
        if (sessionId == Guid.Empty)
        {
            throw new DomainException(
                SecurityErrors.RefreshToken.SessionIdRequired);
        }

        if (tokenHash is null || tokenHash.Length == 0)
        {
            throw new DomainException(
                SecurityErrors.RefreshToken.TokenHashRequired);
        }

        if (expiresAtUtc <= createdAtUtc)
        {
            throw new DomainException(
                SecurityErrors.RefreshToken.ExpiryBeforeCreation);
        }

        SessionId = sessionId;
        TokenHash = tokenHash;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }

    public static RefreshToken Create(
        Guid sessionId,
        byte[] tokenHash,
        DateTime createdAtUtc,
        DateTime expiresAtUtc)
    {
        return new RefreshToken(
            Guid.NewGuid(),
            sessionId,
            tokenHash,
            createdAtUtc,
            expiresAtUtc);
    }

    public void Use(
        byte[] tokenHash,
        DateTime usedAtUtc,
        Guid replacementTokenId)
    {
        if (IsRevoked)
        {
            throw new DomainException(
                SecurityErrors.RefreshToken.Revoked);
        }

        if (IsUsed)
        {
            throw new DomainException(
                SecurityErrors.RefreshToken.AlreadyUsed);
        }

        if (usedAtUtc >= ExpiresAtUtc)
        {
            throw new DomainException(
                SecurityErrors.RefreshToken.Expired);
        }

        if (usedAtUtc < CreatedAtUtc)
        {
            throw new DomainException(
                SecurityErrors.RefreshToken.UsedBeforeCreation);
        }

        if (tokenHash is null || tokenHash.Length == 0)
        {
            throw new DomainException(
                SecurityErrors.RefreshToken.InvalidToken);
        }

        if (!CryptographicOperations.FixedTimeEquals(
                TokenHash,
                tokenHash))
        {
            throw new DomainException(
                SecurityErrors.RefreshToken.InvalidToken);
        }

        if (replacementTokenId == Guid.Empty)
        {
            throw new DomainException(
                SecurityErrors.RefreshToken.ReplacementTokenRequired);
        }

        UsedAtUtc = usedAtUtc;
        ReplacedByTokenId = replacementTokenId;
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
                SecurityErrors.RefreshToken.RevokedBeforeCreation);
        }

        RevokedAtUtc = revokedAtUtc;
    }
}