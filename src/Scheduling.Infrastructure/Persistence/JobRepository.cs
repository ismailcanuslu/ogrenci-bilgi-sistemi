using Microsoft.EntityFrameworkCore;
using Scheduling.Application.Abstractions;
using Scheduling.Domain;

namespace Scheduling.Infrastructure.Persistence;

public sealed class JobRepository(SchedulingDbContext dbContext) : IJobRepository
{
    public async Task AddAsync(JobDefinition job, CancellationToken cancellationToken = default)
    {
        await dbContext.JobDefinitions.AddAsync(job, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<JobDefinition?> GetByIdAsync(JobId id, CancellationToken cancellationToken = default)
    {
        return await dbContext.JobDefinitions
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<JobDefinition>> GetDueJobsAsync(DateTimeOffset nowUtc, CancellationToken cancellationToken = default)
    {
        return await dbContext.JobDefinitions
            .Where(x => x.Status == JobStatus.Scheduled &&
                        x.NextExecutionTimeUtc != null &&
                        x.NextExecutionTimeUtc <= nowUtc)
            .OrderBy(x => x.NextExecutionTimeUtc)
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(JobDefinition job, CancellationToken cancellationToken = default)
    {
        dbContext.JobDefinitions.Update(job);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<JobDefinition> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        JobStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<JobDefinition> query = dbContext.JobDefinitions.AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
