namespace DocumentManagement.Application.Abstractions;

public interface IDocumentManagementUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
