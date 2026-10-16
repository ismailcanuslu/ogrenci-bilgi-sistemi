using System.Security.Cryptography;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Security.Domain.Challenges;

public abstract class Challenge : Entity<Guid>
{
    public Guid UserId { get; protected set; }

    public byte[] CodeHash { get; protected set; } = null!;

    public DateTimeOffset CreatedAtUtc { get; protected set; }

    public DateTimeOffset ExpiresAtUtc { get; protected set; }

    public DateTimeOffset? UsedAtUtc { get; protected set; }

    public int AttemptCount { get; protected set; }

    protected Challenge()
    {
    }

    protected Challenge(
        Guid id,
        Guid userId,
        byte[] codeHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
        : base(id)
    {
        UserId = userId;
        CodeHash = codeHash;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }

    protected void VerifyCode(
        byte[] codeHash,
        DateTimeOffset verifiedAtUtc,
        int maxAttempts)
    {
        EnsureVerifiable(
            verifiedAtUtc,
            maxAttempts);

        if (codeHash is null || codeHash.Length == 0)
        {
            throw new DomainException(
                InvalidCodeErrorCode);
        }

        AttemptCount++;

        if (!CryptographicOperations.FixedTimeEquals(
                CodeHash,
                codeHash))
        {
            if (AttemptCount >= maxAttempts)
            {
                throw new DomainException(
                    MaxAttemptsExceededErrorCode);
            }

            throw new DomainException(
                InvalidCodeErrorCode);
        }

        UsedAtUtc = verifiedAtUtc;
    }

    protected abstract string InvalidCodeErrorCode { get; }

    protected abstract string MaxAttemptsExceededErrorCode { get; }

    protected abstract string ExpiredErrorCode { get; }

    protected abstract string AlreadyUsedErrorCode { get; }

    private void EnsureVerifiable(
        DateTimeOffset now,
        int maxAttempts)
    {
        if (maxAttempts <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxAttempts));
        }

        if (UsedAtUtc is not null)
        {
            throw new DomainException(
                AlreadyUsedErrorCode);
        }

        if (now >= ExpiresAtUtc)
        {
            throw new DomainException(
                ExpiredErrorCode);
        }

        if (AttemptCount >= maxAttempts)
        {
            throw new DomainException(
                MaxAttemptsExceededErrorCode);
        }
    }
}