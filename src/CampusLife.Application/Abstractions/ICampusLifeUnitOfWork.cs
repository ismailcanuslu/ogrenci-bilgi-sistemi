namespace CampusLife.Application.Abstractions;

public interface ICampusLifeUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
