using ImportExport.Domain;

namespace ImportExport.Application.Abstractions;

public interface IImportJobRepository
{
    Task<ImportJob?> GetByIdAsync(ImportJobId id, CancellationToken ct = default);
    Task<IReadOnlyList<ImportJob>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(ImportJob job, CancellationToken ct = default);
    Task UpdateAsync(ImportJob job, CancellationToken ct = default);
}
