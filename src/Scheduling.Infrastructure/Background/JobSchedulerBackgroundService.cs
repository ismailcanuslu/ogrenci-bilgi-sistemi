using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scheduling.Application.Abstractions;
using Scheduling.Domain;

namespace Scheduling.Infrastructure.Background;

public sealed class JobSchedulerBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<JobSchedulerBackgroundService> logger)
    : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new("OBS.Scheduling.JobScheduler", "1.0.0");
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Job Scheduler Background Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessDueJobsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred during job scheduling cycle.");
            }

            try
            {
                await Task.Delay(PollingInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        logger.LogInformation("Job Scheduler Background Service stopped.");
    }

    public async Task<int> ProcessDueJobsAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
        var registeredJobs = scope.ServiceProvider.GetServices<IJob>().ToDictionary(j => j.Name, StringComparer.OrdinalIgnoreCase);

        var now = DateTimeOffset.UtcNow;
        var dueJobs = await repository.GetDueJobsAsync(now, cancellationToken);

        if (dueJobs.Count == 0)
        {
            return 0;
        }

        logger.LogDebug("Found {Count} due jobs to execute.", dueJobs.Count);

        foreach (var job in dueJobs)
        {
            using var activity = ActivitySource.StartActivity("ExecuteScheduledJob", ActivityKind.Internal);
            activity?.SetTag("scheduling.job.id", job.Id.Value.ToString());
            activity?.SetTag("scheduling.job.name", job.Name);

            job.MarkRunning();
            await repository.UpdateAsync(job, cancellationToken);

            try
            {
                if (registeredJobs.TryGetValue(job.Name, out var handler))
                {
                    await handler.ExecuteAsync(job.PayloadJson, cancellationToken);
                }
                else
                {
                    logger.LogWarning("No handler registered for scheduled job name {Name}.", job.Name);
                }

                DateTimeOffset? nextRun = !string.IsNullOrWhiteSpace(job.CronExpression)
                    ? now.AddHours(24) // Default daily recurring if cron parser not attached
                    : null;

                job.MarkCompleted(DateTimeOffset.UtcNow, nextRun);
                activity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Execution failed for job {Name} ({Id}).", job.Name, job.Id.Value);
                job.MarkFailed(ex.Message, now.AddMinutes(5));
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            }

            await repository.UpdateAsync(job, cancellationToken);
        }

        return dueJobs.Count;
    }
}
