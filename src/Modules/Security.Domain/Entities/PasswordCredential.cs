using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;
using Security.Domain.Errors;

namespace Security.Domain.Entities;

public sealed class PasswordCredential : Entity<Guid>
{
    public Guid UserId { get; private set; }

    public string PasswordHash { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? LastUsedAtUtc { get; private set; }

    public DateTime PasswordChangedAtUtc { get; private set; }

    private PasswordCredential()
    {
    }

    private PasswordCredential(
        Guid id,
        Guid userId,
        string passwordHash,
        DateTime createdAtUtc)
        : base(id)
    {
        if (userId == Guid.Empty)
            throw new DomainException(
                SecurityErrors.PasswordCredential.UserIdRequired);

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException(
                SecurityErrors.PasswordCredential.PasswordHashRequired);

        UserId = userId;
        PasswordHash = passwordHash;
        CreatedAtUtc = createdAtUtc;
        PasswordChangedAtUtc = createdAtUtc;
    }

    public static PasswordCredential Create(
        Guid userId,
        string passwordHash,
        DateTime createdAtUtc)
        => new(
            Guid.NewGuid(),
            userId,
            passwordHash,
            createdAtUtc);

    public void ChangePassword(
        string passwordHash,
        DateTime changedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException(
                SecurityErrors.PasswordCredential.PasswordHashRequired);

        PasswordHash = passwordHash;
        PasswordChangedAtUtc = changedAtUtc;
    }

    public void MarkAsUsed(DateTime usedAtUtc)
    {
        if (usedAtUtc < CreatedAtUtc)
            throw new DomainException(
                SecurityErrors.PasswordCredential.UsedBeforeCreation);

        LastUsedAtUtc = usedAtUtc;
    }
}