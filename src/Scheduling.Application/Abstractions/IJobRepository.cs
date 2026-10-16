using Scheduling.Domain;

namespace Scheduling.Application.Abstractions;

public interface IJobRepository
{
    Task AddAsync(JobDefinition job, CancellationToken cancellationToken = default);

    Task<JobDefinition?> GetByIdAsync(JobId id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<JobDefinition>> GetDueJobsAsync(DateTimeOffset nowUtc, CancellationToken cancellationToken = default);

    Task UpdateAsync(JobDefinition job, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<JobDefinition> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        JobStatus? status = null,
        CancellationToken cancellationToken = default);
}
