using BuildingBlocks.Domain.Exceptions;
using Security.Domain.Errors;

namespace Security.Domain.Challenges;

public sealed class EmailVerificationChallenge : Challenge
{
    protected override string InvalidCodeErrorCode =>
        SecurityErrors.EmailVerificationChallenge.InvalidCode;

    protected override string MaxAttemptsExceededErrorCode =>
        SecurityErrors.EmailVerificationChallenge.MaxAttemptsExceeded;

    protected override string ExpiredErrorCode =>
        SecurityErrors.EmailVerificationChallenge.Expired;

    protected override string AlreadyUsedErrorCode =>
        SecurityErrors.EmailVerificationChallenge.AlreadyUsed;

    private EmailVerificationChallenge()
    {
    }

    private EmailVerificationChallenge(
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
                SecurityErrors.EmailVerificationChallenge.UserIdRequired);
        }

        if (codeHash is null || codeHash.Length == 0)
        {
            throw new DomainException(
                SecurityErrors.EmailVerificationChallenge.CodeHashRequired);
        }

        if (expiresAtUtc <= createdAtUtc)
        {
            throw new DomainException(
                SecurityErrors.EmailVerificationChallenge.ExpiryBeforeCreation);
        }
    }

    public static EmailVerificationChallenge Create(
        Guid userId,
        byte[] codeHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        return new EmailVerificationChallenge(
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