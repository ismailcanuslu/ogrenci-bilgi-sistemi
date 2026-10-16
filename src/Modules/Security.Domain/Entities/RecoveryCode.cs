using System.Security.Cryptography;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;
using Security.Domain.Errors;

namespace Security.Domain.Entities;

public sealed class RecoveryCode : Entity<Guid>
{
    public Guid UserId { get; private set; }

    public byte[] CodeHash { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UsedAtUtc { get; private set; }

    public bool IsUsed => UsedAtUtc is not null;

    private RecoveryCode()
    {
    }

    private RecoveryCode(
        Guid id,
        Guid userId,
        byte[] codeHash,
        DateTime createdAtUtc)
        : base(id)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException(
                SecurityErrors.RecoveryCode.UserIdRequired);
        }

        if (codeHash is null || codeHash.Length == 0)
        {
            throw new DomainException(
                SecurityErrors.RecoveryCode.CodeHashRequired);
        }

        UserId = userId;
        CodeHash = codeHash;
        CreatedAtUtc = createdAtUtc;
    }

    public static RecoveryCode Create(
        Guid userId,
        byte[] codeHash,
        DateTime createdAtUtc)
    {
        return new RecoveryCode(
            Guid.NewGuid(),
            userId,
            codeHash,
            createdAtUtc);
    }

    public void Use(
        byte[] codeHash,
        DateTime usedAtUtc)
    {
        if (IsUsed)
        {
            throw new DomainException(
                SecurityErrors.RecoveryCode.AlreadyUsed);
        }

        if (usedAtUtc < CreatedAtUtc)
        {
            throw new DomainException(
                SecurityErrors.RecoveryCode.UsedBeforeCreation);
        }

        if (codeHash is null || codeHash.Length == 0)
        {
            throw new DomainException(
                SecurityErrors.RecoveryCode.InvalidCode);
        }

        if (!CryptographicOperations.FixedTimeEquals(
                CodeHash,
                codeHash))
        {
            throw new DomainException(
                SecurityErrors.RecoveryCode.InvalidCode);
        }

        UsedAtUtc = usedAtUtc;
    }
}