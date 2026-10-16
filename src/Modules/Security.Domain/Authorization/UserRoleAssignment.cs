using BuildingBlocks.Domain.Authorization;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Security.Domain.Authorization;

/// <summary>
/// Bir kullanıcının bir role belirli bir kaynak kapsamında (üniversite/fakülte/bölüm/ders/kendisi)
/// sahip olduğunu belirtir. Kayıtlar silinmez; iptal edilerek geçmiş korunur.
/// </summary>
public sealed class UserRoleAssignment : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }

    public Guid RoleId { get; private set; }

    public ScopeType ScopeType { get; private set; }

    /// <summary>University kapsamında null; diğer kapsamlarda ilgili kaynağın kimliği.</summary>
    public Guid? ScopeId { get; private set; }

    public DateTime AssignedAtUtc { get; private set; }

    public Guid? AssignedByUserId { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    public Guid? RevokedByUserId { get; private set; }

    public string? RevocationReason { get; private set; }

    public bool IsActive => RevokedAtUtc is null;

    private UserRoleAssignment()
    {
    }

    private UserRoleAssignment(
        Guid id,
        Guid userId,
        Guid roleId,
        ScopeType scopeType,
        Guid? scopeId,
        DateTime assignedAtUtc,
        Guid? assignedByUserId)
        : base(id)
    {
        UserId = userId;
        RoleId = roleId;
        ScopeType = scopeType;
        ScopeId = scopeId;
        AssignedAtUtc = assignedAtUtc;
        AssignedByUserId = assignedByUserId;
    }

    public static UserRoleAssignment Create(
        Guid userId,
        Guid roleId,
        ScopeType scopeType,
        Guid? scopeId,
        DateTime assignedAtUtc,
        Guid? assignedByUserId)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException("Security.RoleAssignment.UserRequired");
        }

        if (roleId == Guid.Empty)
        {
            throw new DomainException("Security.RoleAssignment.RoleRequired");
        }

        if (!Enum.IsDefined(scopeType))
        {
            throw new DomainException("Security.RoleAssignment.InvalidScope");
        }

        // University: kimlik olmamalı. Self: kapsam kullanıcının kendisidir, ayrıca saklanmaz.
        var scopeIdRequired = scopeType is ScopeType.Faculty or ScopeType.Department or ScopeType.Course;

        if (scopeIdRequired && (scopeId is null || scopeId == Guid.Empty))
        {
            throw new DomainException("Security.RoleAssignment.ScopeIdRequired");
        }

        if (!scopeIdRequired && scopeId is not null)
        {
            throw new DomainException("Security.RoleAssignment.ScopeIdNotAllowed");
        }

        return new UserRoleAssignment(
            Guid.NewGuid(), userId, roleId, scopeType, scopeId, assignedAtUtc, assignedByUserId);
    }

    public void Revoke(DateTime revokedAtUtc, Guid? revokedByUserId, string? reason)
    {
        if (RevokedAtUtc is not null)
        {
            throw new DomainException("Security.RoleAssignment.AlreadyRevoked");
        }

        RevokedAtUtc = revokedAtUtc;
        RevokedByUserId = revokedByUserId;
        RevocationReason = reason?.Trim();
    }
}
