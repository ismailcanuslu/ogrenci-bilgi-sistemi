using BuildingBlocks.Domain.Exceptions;
using Security.Domain.Errors;

namespace Security.Domain.Challenges;

public sealed class EmailLoginChallenge : Challenge
{
    protected override string InvalidCodeErrorCode =>
        SecurityErrors.EmailLoginChallenge.InvalidCode;

    protected override string MaxAttemptsExceededErrorCode =>
        SecurityErrors.EmailLoginChallenge.MaxAttemptsExceeded;

    protected override string ExpiredErrorCode =>
        SecurityErrors.EmailLoginChallenge.Expired;

    protected override string AlreadyUsedErrorCode =>
        SecurityErrors.EmailLoginChallenge.AlreadyUsed;

    private EmailLoginChallenge()
    {
    }

    private EmailLoginChallenge(
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
                SecurityErrors.EmailLoginChallenge.UserIdRequired);
        }

        if (codeHash is null || codeHash.Length == 0)
        {
            throw new DomainException(
                SecurityErrors.EmailLoginChallenge.CodeHashRequired);
        }

        if (expiresAtUtc <= createdAtUtc)
        {
            throw new DomainException(
                SecurityErrors.EmailLoginChallenge.ExpiryBeforeCreation);
        }
    }

    public static EmailLoginChallenge Create(
        Guid userId,
        byte[] codeHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        return new EmailLoginChallenge(
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