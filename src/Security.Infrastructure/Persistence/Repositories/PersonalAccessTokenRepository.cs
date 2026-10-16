using Microsoft.EntityFrameworkCore;
using Security.Application.Abstractions.Persistence;
using Security.Domain.Authorization;

namespace Security.Infrastructure.Persistence.Repositories;

public sealed class PersonalAccessTokenRepository(SecurityDbContext dbContext) : IPersonalAccessTokenRepository
{
    public Task<PersonalAccessToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.PersonalAccessTokens
            .Include(x => x.Permissions)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<PersonalAccessToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        => dbContext.PersonalAccessTokens
            .Include(x => x.Permissions)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

    public async Task AddAsync(PersonalAccessToken token, CancellationToken cancellationToken = default)
        => await dbContext.PersonalAccessTokens.AddAsync(token, cancellationToken);

    public async Task<IReadOnlyCollection<PersonalAccessToken>> ListAsync(
        bool activeOnly, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;
        var query = dbContext.PersonalAccessTokens
            .Include(x => x.Permissions)
            .AsNoTracking()
            .AsQueryable();

        if (activeOnly)
        {
            query = query.Where(x => x.RevokedAtUtc == null && (x.ExpiresAtUtc == null || x.ExpiresAtUtc > nowUtc));
        }

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
