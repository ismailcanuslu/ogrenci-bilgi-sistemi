using Security.Domain.Challenges;

namespace Security.Application.Abstractions.Persistence;

public interface IPasskeyAuthenticationChallengeRepository
{
    Task AddAsync(
        PasskeyAuthenticationChallenge challenge,
        CancellationToken cancellationToken);

    Task<PasskeyAuthenticationChallenge?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
}