using Security.Domain.Entities;
using Security.Domain.ValueObjects;

namespace Security.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken);
    
    Task<User?> GetByPasskeyCredentialIdAsync(
        byte[] credentialId,
        CancellationToken cancellationToken);
    
    Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task AddAsync(
        User user,
        CancellationToken cancellationToken);

    void Update(User user);
}