using BuildingBlocks.Domain.Authorization;
using Microsoft.EntityFrameworkCore;
using Security.Application.Abstractions.Persistence;
using Security.Domain.Authorization;

namespace Security.Infrastructure.Persistence.Repositories;

public sealed class UserRoleAssignmentRepository(SecurityDbContext dbContext)
    : IUserRoleAssignmentRepository
{
    public Task<UserRoleAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.UserRoleAssignments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsActiveAsync(
        Guid userId,
        Guid roleId,
        ScopeType scopeType,
        Guid? scopeId,
        CancellationToken cancellationToken = default)
        => dbContext.UserRoleAssignments.AnyAsync(
            x => x.UserId == userId
                 && x.RoleId == roleId
                 && x.ScopeType == scopeType
                 && x.ScopeId == scopeId
                 && x.RevokedAtUtc == null,
            cancellationToken);

    public async Task AddAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default)
        => await dbContext.UserRoleAssignments.AddAsync(assignment, cancellationToken);

    public async Task<IReadOnlyList<string>> GetRoleNamesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var roleIds = await dbContext.UserRoleAssignments
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null)
            .Select(x => x.RoleId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return await dbContext.Roles
            .Where(r => roleIds.Contains(r.Id))
            .Select(r => r.Name)
            .ToListAsync(cancellationToken);
    }
}

