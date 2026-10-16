using BuildingBlocks.Domain.Exceptions;
using Security.Domain.Errors;

namespace Security.Domain.Challenges;

public sealed class PasswordResetChallenge : Challenge
{
    protected override string InvalidCodeErrorCode =>
        SecurityErrors.PasswordResetChallenge.InvalidCode;

    protected override string MaxAttemptsExceededErrorCode =>
        SecurityErrors.PasswordResetChallenge.MaxAttemptsExceeded;

    protected override string ExpiredErrorCode =>
        SecurityErrors.PasswordResetChallenge.Expired;

    protected override string AlreadyUsedErrorCode =>
        SecurityErrors.PasswordResetChallenge.AlreadyUsed;

    private PasswordResetChallenge()
    {
    }

    private PasswordResetChallenge(
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
                SecurityErrors.PasswordResetChallenge.UserIdRequired);
        }

        if (codeHash is null || codeHash.Length == 0)
        {
            throw new DomainException(
                SecurityErrors.PasswordResetChallenge.CodeHashRequired);
        }

        if (expiresAtUtc <= createdAtUtc)
        {
            throw new DomainException(
                SecurityErrors.PasswordResetChallenge.ExpiryBeforeCreation);
        }
    }

    public static PasswordResetChallenge Create(
        Guid userId,
        byte[] codeHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        return new PasswordResetChallenge(
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