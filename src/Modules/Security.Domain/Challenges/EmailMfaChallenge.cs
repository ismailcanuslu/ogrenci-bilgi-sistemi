using BuildingBlocks.Domain.Exceptions;
using Security.Domain.Errors;

namespace Security.Domain.Challenges;

public sealed class EmailMfaChallenge : Challenge
{
    protected override string InvalidCodeErrorCode =>
        SecurityErrors.EmailMfaChallenge.InvalidCode;

    protected override string MaxAttemptsExceededErrorCode =>
        SecurityErrors.EmailMfaChallenge.MaxAttemptsExceeded;

    protected override string ExpiredErrorCode =>
        SecurityErrors.EmailMfaChallenge.Expired;

    protected override string AlreadyUsedErrorCode =>
        SecurityErrors.EmailMfaChallenge.AlreadyUsed;

    private EmailMfaChallenge()
    {
    }

    private EmailMfaChallenge(
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
                SecurityErrors.EmailMfaChallenge.UserIdRequired);
        }

        if (codeHash is null || codeHash.Length == 0)
        {
            throw new DomainException(
                SecurityErrors.EmailMfaChallenge.CodeHashRequired);
        }

        if (expiresAtUtc <= createdAtUtc)
        {
            throw new DomainException(
                SecurityErrors.EmailMfaChallenge.ExpiryBeforeCreation);
        }
    }

    public static EmailMfaChallenge Create(
        Guid userId,
        byte[] codeHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        return new EmailMfaChallenge(
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