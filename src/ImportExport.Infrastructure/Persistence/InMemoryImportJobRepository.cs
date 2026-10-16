using System.Collections.Concurrent;
using ImportExport.Application.Abstractions;
using ImportExport.Domain;

namespace ImportExport.Infrastructure.Persistence;

public sealed class InMemoryImportJobRepository : IImportJobRepository
{
    private readonly ConcurrentDictionary<Guid, ImportJob> _jobs = new();

    public Task<ImportJob?> GetByIdAsync(ImportJobId id, CancellationToken ct = default)
    {
        _jobs.TryGetValue(id.Value, out var job);
        return Task.FromResult(job);
    }

    public Task<IReadOnlyList<ImportJob>> GetAllAsync(CancellationToken ct = default)
    {
        var list = _jobs.Values.OrderByDescending(j => j.CreatedAtUtc).ToList();
        return Task.FromResult<IReadOnlyList<ImportJob>>(list);
    }

    public Task AddAsync(ImportJob job, CancellationToken ct = default)
    {
        _jobs[job.Id.Value] = job;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ImportJob job, CancellationToken ct = default)
    {
        _jobs[job.Id.Value] = job;
        return Task.CompletedTask;
    }
}
