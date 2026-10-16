using Security.Domain.Authorization;

namespace Security.Application.Abstractions.Persistence;

public interface IPersonalAccessTokenRepository
{
    Task<PersonalAccessToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PersonalAccessToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task AddAsync(PersonalAccessToken token, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PersonalAccessToken>> ListAsync(
        bool activeOnly, int page, int pageSize, CancellationToken cancellationToken = default);
}
