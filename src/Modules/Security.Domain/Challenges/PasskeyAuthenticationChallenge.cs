using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;
using Security.Domain.Errors;

namespace Security.Domain.Challenges;

public sealed class PasskeyAuthenticationChallenge : Entity<Guid>
{
    public Guid UserId { get; private set; }

    public string OptionsJson { get; private set; } = null!;

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset ExpiresAtUtc { get; private set; }

    public DateTimeOffset? UsedAtUtc { get; private set; }

    public bool IsUsed =>
        UsedAtUtc is not null;

    public bool IsExpired(DateTimeOffset nowUtc) =>
        nowUtc >= ExpiresAtUtc;

    private PasskeyAuthenticationChallenge()
    {
    }

    private PasskeyAuthenticationChallenge(
        Guid id,
        Guid userId,
        string optionsJson,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
        : base(id)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException(
                SecurityErrors.PasskeyAuthenticationChallenge.UserIdRequired);
        }

        if (string.IsNullOrWhiteSpace(optionsJson))
        {
            throw new DomainException(
                SecurityErrors.PasskeyAuthenticationChallenge.OptionsRequired);
        }

        if (expiresAtUtc <= createdAtUtc)
        {
            throw new DomainException(
                SecurityErrors.PasskeyAuthenticationChallenge.ExpiryBeforeCreation);
        }

        UserId = userId;
        OptionsJson = optionsJson;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }

    public static PasskeyAuthenticationChallenge Create(
        Guid userId,
        string optionsJson,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        return new PasskeyAuthenticationChallenge(
            Guid.NewGuid(),
            userId,
            optionsJson,
            createdAtUtc,
            expiresAtUtc);
    }

    public void MarkAsUsed(DateTimeOffset usedAtUtc)
    {
        if (UsedAtUtc is not null)
        {
            throw new DomainException(
                SecurityErrors.PasskeyAuthenticationChallenge.AlreadyUsed);
        }

        if (usedAtUtc >= ExpiresAtUtc)
        {
            throw new DomainException(
                SecurityErrors.PasskeyAuthenticationChallenge.Expired);
        }

        if (usedAtUtc < CreatedAtUtc)
        {
            throw new DomainException(
                SecurityErrors.PasskeyAuthenticationChallenge.UsedBeforeCreation);
        }

        UsedAtUtc = usedAtUtc;
    }
}