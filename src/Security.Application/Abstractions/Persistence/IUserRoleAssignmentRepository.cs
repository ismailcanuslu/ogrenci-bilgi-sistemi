using BuildingBlocks.Domain.Authorization;
using Security.Domain.Authorization;

namespace Security.Application.Abstractions.Persistence;

public interface IUserRoleAssignmentRepository
{
    Task<UserRoleAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsActiveAsync(
        Guid userId,
        Guid roleId,
        ScopeType scopeType,
        Guid? scopeId,
        CancellationToken cancellationToken = default);

    Task AddAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRoleNamesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
