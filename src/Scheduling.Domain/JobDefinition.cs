using BuildingBlocks.Domain.Primitives;

namespace Scheduling.Domain;

public sealed class JobDefinition : Entity<JobId>
{
    private JobDefinition(
        JobId id,
        string name,
        string? description,
        string? cronExpression,
        string? payloadJson,
        JobStatus status,
        DateTimeOffset? nextExecutionTimeUtc,
        DateTimeOffset createdAtUtc,
        int maxRetries)
        : base(id)
    {
        Name = name;
        Description = description;
        CronExpression = cronExpression;
        PayloadJson = payloadJson;
        Status = status;
        NextExecutionTimeUtc = nextExecutionTimeUtc;
        CreatedAtUtc = createdAtUtc;
        MaxRetries = maxRetries;
        RetryCount = 0;
    }

    private JobDefinition()
    {
    }

    public string Name { get; private set; } = default!;

    public string? Description { get; private set; }

    public string? CronExpression { get; private set; }

    public string? PayloadJson { get; private set; }

    public JobStatus Status { get; private set; }

    public DateTimeOffset? NextExecutionTimeUtc { get; private set; }

    public DateTimeOffset? LastExecutedTimeUtc { get; private set; }

    public int RetryCount { get; private set; }

    public int MaxRetries { get; private set; }

    public string? ErrorMessage { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static JobDefinition Create(
        string name,
        string? description = null,
        string? cronExpression = null,
        DateTimeOffset? nextExecutionTimeUtc = null,
        string? payloadJson = null,
        int maxRetries = 3)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new JobDefinition(
            JobId.New(),
            name,
            description,
            cronExpression,
            payloadJson,
            JobStatus.Scheduled,
            nextExecutionTimeUtc ?? DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            maxRetries);
    }

    public void MarkRunning()
    {
        Status = JobStatus.Running;
    }

    public void MarkCompleted(DateTimeOffset completedAt, DateTimeOffset? nextRunUtc = null)
    {
        LastExecutedTimeUtc = completedAt;
        ErrorMessage = null;
        RetryCount = 0;

        if (nextRunUtc.HasValue)
        {
            NextExecutionTimeUtc = nextRunUtc.Value;
            Status = JobStatus.Scheduled;
        }
        else
        {
            NextExecutionTimeUtc = null;
            Status = JobStatus.Completed;
        }
    }

    public void MarkFailed(string error, DateTimeOffset? nextRetryUtc = null)
    {
        RetryCount++;
        ErrorMessage = error;

        if (RetryCount <= MaxRetries && nextRetryUtc.HasValue)
        {
            NextExecutionTimeUtc = nextRetryUtc.Value;
            Status = JobStatus.Scheduled;
        }
        else
        {
            Status = JobStatus.Failed;
            NextExecutionTimeUtc = null;
        }
    }

    public void Cancel()
    {
        Status = JobStatus.Cancelled;
        NextExecutionTimeUtc = null;
    }

    public void SetNextExecution(DateTimeOffset nextUtc)
    {
        NextExecutionTimeUtc = nextUtc;
        Status = JobStatus.Scheduled;
    }
}
