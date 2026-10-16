using Microsoft.EntityFrameworkCore;
using Security.Application.Abstractions.Persistence;
using Security.Domain.Authorization;

namespace Security.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository(SecurityDbContext dbContext) : IRoleRepository
{
    public Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Roles
            .Include(x => x.Permissions)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => dbContext.Roles
            .Include(x => x.Permissions)
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
        => await dbContext.Roles.AddAsync(role, cancellationToken);
}
