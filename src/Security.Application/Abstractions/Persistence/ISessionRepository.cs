using Security.Domain.Entities;

namespace Security.Application.Abstractions.Persistence;

public interface ISessionRepository
{
    Task<Session?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Session?> GetByRefreshTokenHashAsync(
        byte[] tokenHash,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Session session,
        CancellationToken cancellationToken = default);

    void Update(Session session);
}