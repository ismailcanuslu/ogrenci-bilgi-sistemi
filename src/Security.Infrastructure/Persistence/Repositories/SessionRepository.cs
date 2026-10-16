using Microsoft.EntityFrameworkCore;
using Security.Application.Abstractions.Persistence;
using Security.Domain.Entities;

namespace Security.Infrastructure.Persistence.Repositories;

public sealed class SessionRepository : ISessionRepository
{
    private readonly SecurityDbContext _dbContext;

    public SessionRepository(SecurityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Session?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Session?> GetByRefreshTokenHashAsync(
        byte[] tokenHash,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(
                x => x.RefreshTokens.Any(rt => rt.TokenHash == tokenHash),
                cancellationToken);
    }

    public async Task AddAsync(
        Session session,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Sessions.AddAsync(
            session,
            cancellationToken);
    }

    public void Update(Session session)
    {
        _dbContext.Sessions.Update(session);
    }
}