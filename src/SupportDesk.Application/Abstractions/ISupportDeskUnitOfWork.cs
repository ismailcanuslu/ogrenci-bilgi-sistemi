namespace SupportDesk.Application.Abstractions;

public interface ISupportDeskUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
