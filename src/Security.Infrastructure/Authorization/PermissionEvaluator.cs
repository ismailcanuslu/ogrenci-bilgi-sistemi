using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Domain.Authorization;
using Microsoft.EntityFrameworkCore;
using Security.Domain.Authorization;
using Security.Infrastructure.Persistence;

namespace Security.Infrastructure.Authorization;

/// <summary>
/// Yetkiyi her istekte veritabanından güncel okur; böylece JWT içinde kapsam taşımaya
/// gerek kalmaz ve iptal edilen atamalar/pasifleştirilen kullanıcılar hemen etkisiz olur.
/// </summary>
public sealed class PermissionEvaluator(SecurityDbContext dbContext) : IPermissionEvaluator
{
    public async Task<bool> HasPermissionAsync(
        Guid userId,
        string permission,
        IReadOnlyCollection<ResourceScope> candidateScopes,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(candidateScopes);

        var grants = await GetGrantedScopesAsync(userId, permission, cancellationToken);

        if (grants.Count == 0)
        {
            return false;
        }

        if (grants.Any(g => g.Type == ScopeType.University))
        {
            return true;
        }

        return candidateScopes.Any(candidate =>
            candidate.Id is not null &&
            grants.Any(g => g.Type == candidate.Type && g.Id == candidate.Id));
    }

    public async Task<IReadOnlyCollection<ResourceScope>> GetGrantedScopesAsync(
        Guid userId,
        string permission,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty || string.IsNullOrWhiteSpace(permission))
        {
            return [];
        }

        var normalizedPermission = permission.Trim().ToLowerInvariant();
        var nowUtc = DateTime.UtcNow;

        // 1. Önce PAT kontrolü: userId bir Personal Access Token ID'si mi?
        var pat = await dbContext.PersonalAccessTokens
            .Include(p => p.Permissions)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == userId, cancellationToken);

        if (pat is not null)
        {
            if (!pat.IsActive(nowUtc))
            {
                return [];
            }

            if (!pat.HasPermission(normalizedPermission))
            {
                return [];
            }

            return pat.ScopeType switch
            {
                ScopeType.University => [ResourceScope.University],
                ScopeType.Faculty when pat.ScopeId.HasValue => [ResourceScope.Faculty(pat.ScopeId.Value)],
                ScopeType.Department when pat.ScopeId.HasValue => [ResourceScope.Department(pat.ScopeId.Value)],
                _ => [new ResourceScope(pat.ScopeType, pat.ScopeId)]
            };
        }

        // 2. Normal kullanıcı kontrolü
        var userIsUsable = await dbContext.Users
            .AsNoTracking()
            .AnyAsync(
                u => u.Id == userId
                     && u.IsActive
                     && (u.LockedUntilUtc == null || u.LockedUntilUtc <= nowUtc),
                cancellationToken);

        if (!userIsUsable)
        {
            return [];
        }

        var rows = await (
                from assignment in dbContext.UserRoleAssignments.AsNoTracking()
                join rolePermission in dbContext.Set<RolePermission>().AsNoTracking()
                    on assignment.RoleId equals rolePermission.RoleId
                where assignment.UserId == userId
                      && assignment.RevokedAtUtc == null
                      && rolePermission.Permission == normalizedPermission
                select new { assignment.ScopeType, assignment.ScopeId })
            .Distinct()
            .ToListAsync(cancellationToken);

        return rows
            .Select(r => r.ScopeType switch
            {
                ScopeType.University => ResourceScope.University,
                ScopeType.Self => ResourceScope.Self(userId),
                _ => new ResourceScope(r.ScopeType, r.ScopeId)
            })
            .ToArray();
    }
}
