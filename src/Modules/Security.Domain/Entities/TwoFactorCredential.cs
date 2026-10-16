using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;
using Security.Domain.Errors;

namespace Security.Domain.Entities;

public sealed class TotpCredential : Entity<Guid>
{
    public Guid UserId { get; private set; }

    public string ProtectedSecret { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? EnabledAtUtc { get; private set; }

    public DateTime? DisabledAtUtc { get; private set; }

    public DateTime? LastUsedAtUtc { get; private set; }

    public bool IsEnabled =>
        EnabledAtUtc is not null &&
        DisabledAtUtc is null;

    private TotpCredential()
    {
    }

    private TotpCredential(
        Guid id,
        Guid userId,
        string protectedSecret,
        DateTime createdAtUtc)
        : base(id)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException(
                SecurityErrors.TotpCredential.UserIdRequired);
        }

        if (string.IsNullOrWhiteSpace(protectedSecret))
        {
            throw new DomainException(
                SecurityErrors.TotpCredential.ProtectedSecretRequired);
        }

        UserId = userId;
        ProtectedSecret = protectedSecret;
        CreatedAtUtc = createdAtUtc;
    }

    public static TotpCredential Create(
        Guid userId,
        string protectedSecret,
        DateTime createdAtUtc)
    {
        return new TotpCredential(
            Guid.NewGuid(),
            userId,
            protectedSecret,
            createdAtUtc);
    }

    public void Enable(DateTime enabledAtUtc)
    {
        if (enabledAtUtc < CreatedAtUtc)
        {
            throw new DomainException(
                SecurityErrors.TotpCredential.EnabledBeforeCreation);
        }

        if (DisabledAtUtc is not null)
        {
            throw new DomainException(
                SecurityErrors.TotpCredential.Disabled);
        }

        if (EnabledAtUtc is not null)
        {
            throw new DomainException(
                SecurityErrors.TotpCredential.AlreadyEnabled);
        }

        EnabledAtUtc = enabledAtUtc;
    }

    public void RecordUse(DateTime usedAtUtc)
    {
        if (!IsEnabled)
        {
            throw new DomainException(
                SecurityErrors.TotpCredential.NotEnabled);
        }

        if (usedAtUtc < EnabledAtUtc)
        {
            throw new DomainException(
                SecurityErrors.TotpCredential.UsedBeforeEnabled);
        }

        LastUsedAtUtc = usedAtUtc;
    }
    public void RecordSuccessfulVerification(DateTime verifiedAtUtc)
    {
        if (!IsEnabled)
        {
            throw new DomainException(
                SecurityErrors.TotpCredential.NotEnabled);
        }

        if (verifiedAtUtc < EnabledAtUtc)
        {
            throw new DomainException(
                SecurityErrors.TotpCredential.VerifiedBeforeEnabled);
        }

        LastUsedAtUtc = verifiedAtUtc;
    }

    public void Disable(DateTime disabledAtUtc)
    {
        if (!IsEnabled)
        {
            throw new DomainException(
                SecurityErrors.TotpCredential.NotEnabled);
        }

        if (disabledAtUtc < EnabledAtUtc)
        {
            throw new DomainException(
                SecurityErrors.TotpCredential.DisabledBeforeEnabled);
        }

        DisabledAtUtc = disabledAtUtc;
    }
}