using BuildingBlocks.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Security.Application.Abstractions.Persistence;
using Security.Domain.Entities;
using Security.Domain.ValueObjects;

namespace Security.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly SecurityDbContext _dbContext;

    public UserRepository(SecurityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken)
    {
        return _dbContext.Users
            .Include(x => x.PasswordCredential)
            .Include(x => x.PasskeyCredentials)
            .Include(x => x.TotpCredential)
            .Include(x => x.RecoveryCodes)
            .SingleOrDefaultAsync(
                x => x.Email.Value == email.Value,
                cancellationToken);
    }
    
    public Task<User?> GetByPasskeyCredentialIdAsync(
        byte[] credentialId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Users
            .Include(x => x.PasskeyCredentials)
            .SingleOrDefaultAsync(
                x => x.PasskeyCredentials.Any(
                    p => p.CredentialId == credentialId),
                cancellationToken);
    }
    
    public Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return _dbContext.Users
            .Include(x => x.PasskeyCredentials)
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public void Update(User user)
    {
        _dbContext.Users.Update(user);
    }
}