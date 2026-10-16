using Security.Domain.Challenges;

namespace Security.Application.Abstractions.Security;

public interface IPasskeyRegistrationChallengeRepository
{
    Task AddAsync(
        PasskeyRegistrationChallenge challenge,
        CancellationToken cancellationToken);

    Task<PasskeyRegistrationChallenge?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
}