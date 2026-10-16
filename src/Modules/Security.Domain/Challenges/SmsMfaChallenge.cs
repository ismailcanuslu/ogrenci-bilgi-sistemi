using BuildingBlocks.Domain.Exceptions;
using Security.Domain.Errors;

namespace Security.Domain.Challenges;

public sealed class SmsMfaChallenge : Challenge
{
    protected override string InvalidCodeErrorCode =>
        SecurityErrors.SmsMfaChallenge.InvalidCode;

    protected override string MaxAttemptsExceededErrorCode =>
        SecurityErrors.SmsMfaChallenge.MaxAttemptsExceeded;

    protected override string ExpiredErrorCode =>
        SecurityErrors.SmsMfaChallenge.Expired;

    protected override string AlreadyUsedErrorCode =>
        SecurityErrors.SmsMfaChallenge.AlreadyUsed;

    private SmsMfaChallenge()
    {
    }

    private SmsMfaChallenge(
        Guid id,
        Guid userId,
        byte[] codeHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
        : base(
            id,
            userId,
            codeHash,
            createdAtUtc,
            expiresAtUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException(
                SecurityErrors.SmsMfaChallenge.UserIdRequired);
        }

        if (codeHash is null || codeHash.Length == 0)
        {
            throw new DomainException(
                SecurityErrors.SmsMfaChallenge.CodeHashRequired);
        }

        if (expiresAtUtc <= createdAtUtc)
        {
            throw new DomainException(
                SecurityErrors.SmsMfaChallenge.ExpiryBeforeCreation);
        }
    }

    public static SmsMfaChallenge Create(
        Guid userId,
        byte[] codeHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        return new SmsMfaChallenge(
            Guid.NewGuid(),
            userId,
            codeHash,
            createdAtUtc,
            expiresAtUtc);
    }

    public void Verify(
        byte[] codeHash,
        DateTimeOffset verifiedAtUtc,
        int maxAttempts)
    {
        VerifyCode(
            codeHash,
            verifiedAtUtc,
            maxAttempts);
    }
}