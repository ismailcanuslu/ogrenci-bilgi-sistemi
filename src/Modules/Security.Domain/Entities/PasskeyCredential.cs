using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;
using Security.Domain.Enums;
using Security.Domain.Errors;

namespace Security.Domain.Entities;

public sealed class PasskeyCredential : Entity<Guid>
{
    public Guid UserId { get; private set; }

    public byte[] CredentialId { get; private set; } = [];

    public byte[] PublicKey { get; private set; } = [];
    
    public byte[] UserHandle { get; private set; } = [];

    public uint SignCount { get; private set; }

    public string? Name { get; private set; }

    public AuthenticatorType AuthenticatorType { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? LastUsedAt { get; private set; }

    public bool IsMfaEnabled { get; private set; }

    public bool IsRevoked { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    private PasskeyCredential()
    {
    }

    private PasskeyCredential(
        Guid id,
        Guid userId,
        byte[] credentialId,
        byte[] publicKey,
        byte[] userHandle,
        AuthenticatorType authenticatorType,
        string? name,
        DateTimeOffset createdAt)
        : base(id)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException(
                SecurityErrors.PasskeyCredential.UserIdRequired);
        }

        if (credentialId is null || credentialId.Length == 0)
        {
            throw new DomainException(
                SecurityErrors.PasskeyCredential.CredentialIdRequired);
        }

        if (publicKey is null || publicKey.Length == 0)
        {
            throw new DomainException(
                SecurityErrors.PasskeyCredential.PublicKeyRequired);
        }

        UserId = userId;
        CredentialId = credentialId.ToArray();
        PublicKey = publicKey.ToArray();
        AuthenticatorType = authenticatorType;
        Name = NormalizeName(name);
        CreatedAt = createdAt;
        UserHandle = userHandle.ToArray();
    }

    public static PasskeyCredential Create(
        Guid userId,
        byte[] credentialId,
        byte[] publicKey,
        byte[] userHandle,
        AuthenticatorType authenticatorType,
        string? name = null)
    {
        return new PasskeyCredential(
            Guid.NewGuid(),
            userId,
            credentialId,
            publicKey,
            userHandle,
            authenticatorType,
            name,
            DateTimeOffset.UtcNow);
    }

    public void Rename(string? name)
    {
        EnsureNotRevoked();

        Name = NormalizeName(name);
    }

    public void EnableMfa()
    {
        EnsureNotRevoked();

        IsMfaEnabled = true;
    }

    public void DisableMfa()
    {
        EnsureNotRevoked();

        IsMfaEnabled = false;
    }

    public PasskeyAuthenticationResult RecordAuthentication(
        uint signCount,
        DateTimeOffset authenticatedAt)
    {
        EnsureNotRevoked();

        if (authenticatedAt < CreatedAt)
        {
            throw new DomainException(
                SecurityErrors.PasskeyCredential.AuthenticationBeforeCreation);
        }

        var signCountAnomaly =
            SignCount != 0 &&
            signCount != 0 &&
            signCount < SignCount;

        SignCount = signCount;
        LastUsedAt = authenticatedAt;

        return signCountAnomaly
            ? PasskeyAuthenticationResult.SignCountAnomaly
            : PasskeyAuthenticationResult.Success;
    }

    public void Revoke(DateTimeOffset revokedAt)
    {
        if (IsRevoked)
        {
            return;
        }

        if (revokedAt < CreatedAt)
        {
            throw new DomainException(
                SecurityErrors.PasskeyCredential.RevocationBeforeCreation);
        }

        IsRevoked = true;
        RevokedAt = revokedAt;
    }

    private void EnsureNotRevoked()
    {
        if (IsRevoked)
        {
            throw new DomainException(
                SecurityErrors.PasskeyCredential.Revoked);
        }
    }

    private static string? NormalizeName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return name.Trim();
    }
}