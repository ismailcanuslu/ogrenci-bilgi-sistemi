using Microsoft.EntityFrameworkCore;
using Security.Application.Abstractions.Security;
using Security.Domain.Challenges;

namespace Security.Infrastructure.Persistence.Repositories;

public sealed class PasskeyRegistrationChallengeRepository(
    SecurityDbContext dbContext)
    : IPasskeyRegistrationChallengeRepository
{
    public async Task AddAsync(
        PasskeyRegistrationChallenge challenge,
        CancellationToken cancellationToken)
    {
        await dbContext.PasskeyRegistrationChallenges.AddAsync(
            challenge,
            cancellationToken);
    }

    public Task<PasskeyRegistrationChallenge?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return dbContext.PasskeyRegistrationChallenges
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }
}