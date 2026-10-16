using Scheduling.Domain;

namespace Scheduling.Application.Abstractions;

public interface ISchedulerService
{
    Task<JobId> ScheduleJobAsync(
        string name,
        string? description = null,
        string? cronExpression = null,
        TimeSpan? delay = null,
        string? payloadJson = null,
        CancellationToken cancellationToken = default);

    Task<bool> TriggerJobNowAsync(JobId jobId, CancellationToken cancellationToken = default);

    Task<bool> CancelJobAsync(JobId jobId, CancellationToken cancellationToken = default);
}
