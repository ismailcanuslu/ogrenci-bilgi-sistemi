using Microsoft.EntityFrameworkCore;
using Security.Application.Abstractions.Persistence;
using Security.Domain.Challenges;

namespace Security.Infrastructure.Persistence.Repositories;

public sealed class PasskeyAuthenticationChallengeRepository(
    SecurityDbContext dbContext)
    : IPasskeyAuthenticationChallengeRepository
{
    public async Task AddAsync(
        PasskeyAuthenticationChallenge challenge,
        CancellationToken cancellationToken)
    {
        await dbContext.PasskeyAuthenticationChallenges
            .AddAsync(challenge, cancellationToken);
    }

    public Task<PasskeyAuthenticationChallenge?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return dbContext.PasskeyAuthenticationChallenges
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }
}