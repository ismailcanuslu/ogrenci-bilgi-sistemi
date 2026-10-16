using System.Diagnostics;
using Scheduling.Application.Abstractions;
using Scheduling.Domain;

namespace Scheduling.Infrastructure.Services;

public sealed class SchedulerService(IJobRepository repository) : ISchedulerService
{
    public async Task<JobId> ScheduleJobAsync(
        string name,
        string? description = null,
        string? cronExpression = null,
        TimeSpan? delay = null,
        string? payloadJson = null,
        CancellationToken cancellationToken = default)
    {
        var nextRun = DateTimeOffset.UtcNow.Add(delay ?? TimeSpan.Zero);

        var job = JobDefinition.Create(
            name: name,
            description: description,
            cronExpression: cronExpression,
            nextExecutionTimeUtc: nextRun,
            payloadJson: payloadJson);

        await repository.AddAsync(job, cancellationToken);

        var activity = Activity.Current;
        activity?.AddEvent(new ActivityEvent("JobScheduled", tags: new ActivityTagsCollection
        {
            ["scheduling.job.id"] = job.Id.Value.ToString(),
            ["scheduling.job.name"] = name
        }));

        return job.Id;
    }

    public async Task<bool> TriggerJobNowAsync(JobId jobId, CancellationToken cancellationToken = default)
    {
        var job = await repository.GetByIdAsync(jobId, cancellationToken);
        if (job is null)
        {
            return false;
        }

        job.SetNextExecution(DateTimeOffset.UtcNow);
        await repository.UpdateAsync(job, cancellationToken);
        return true;
    }

    public async Task<bool> CancelJobAsync(JobId jobId, CancellationToken cancellationToken = default)
    {
        var job = await repository.GetByIdAsync(jobId, cancellationToken);
        if (job is null)
        {
            return false;
        }

        job.Cancel();
        await repository.UpdateAsync(job, cancellationToken);
        return true;
    }
}
