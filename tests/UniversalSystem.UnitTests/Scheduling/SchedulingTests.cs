using NSubstitute;
using Scheduling.Application.Abstractions;
using Scheduling.Application.Features;
using Scheduling.Domain;
using Scheduling.Infrastructure.Services;
using Xunit;

namespace UniversalSystem.UnitTests.Scheduling;

public class SchedulingTests
{
    [Fact]
    public void JobDefinition_Create_And_Lifecycle_ShouldWork()
    {
        // Arrange
        var job = JobDefinition.Create(
            name: "CleanExpiredSessionsJob",
            description: "Cleans sessions older than 30 days",
            cronExpression: "0 2 * * *",
            nextExecutionTimeUtc: DateTimeOffset.UtcNow.AddHours(2),
            payloadJson: "{\"batchSize\":500}");

        // Assert Initial
        Assert.NotNull(job);
        Assert.Equal("CleanExpiredSessionsJob", job.Name);
        Assert.Equal(JobStatus.Scheduled, job.Status);
        Assert.Equal(0, job.RetryCount);

        // Act - Mark Running
        job.MarkRunning();
        Assert.Equal(JobStatus.Running, job.Status);

        // Act - Mark Failed with Retry
        var retryAt = DateTimeOffset.UtcNow.AddMinutes(5);
        job.MarkFailed("Database connection timeout", retryAt);
        Assert.Equal(1, job.RetryCount);
        Assert.Equal(JobStatus.Scheduled, job.Status);
        Assert.Equal(retryAt, job.NextExecutionTimeUtc);
        Assert.Equal("Database connection timeout", job.ErrorMessage);

        // Act - Mark Completed with Next Run
        var completedAt = DateTimeOffset.UtcNow;
        var nextRun = DateTimeOffset.UtcNow.AddDays(1);
        job.MarkCompleted(completedAt, nextRun);
        Assert.Equal(JobStatus.Scheduled, job.Status);
        Assert.Equal(0, job.RetryCount);
        Assert.Null(job.ErrorMessage);
        Assert.Equal(completedAt, job.LastExecutedTimeUtc);
        Assert.Equal(nextRun, job.NextExecutionTimeUtc);

        // Act - Cancel
        job.Cancel();
        Assert.Equal(JobStatus.Cancelled, job.Status);
        Assert.Null(job.NextExecutionTimeUtc);
    }

    [Fact]
    public async Task SchedulerService_ScheduleJob_ShouldPersistAndReturnId()
    {
        // Arrange
        var repository = Substitute.For<IJobRepository>();
        var service = new SchedulerService(repository);

        // Act
        var jobId = await service.ScheduleJobAsync(
            name: "DailyBackupJob",
            cronExpression: "0 0 * * *",
            delay: TimeSpan.FromMinutes(10));

        // Assert
        Assert.NotEqual(Guid.Empty, jobId.Value);
        await repository.Received(1).AddAsync(
            Arg.Is<JobDefinition>(j => j.Name == "DailyBackupJob"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetScheduledJobs_Handler_ShouldReturnPagedResults()
    {
        // Arrange
        var repository = Substitute.For<IJobRepository>();
        var sampleJob = JobDefinition.Create("SampleJob");

        repository.GetPagedAsync(1, 10, null, Arg.Any<CancellationToken>())
            .Returns((new List<JobDefinition> { sampleJob }, 1));

        var handler = new GetScheduledJobs.Handler(repository);
        var query = new GetScheduledJobs.Query(1, 10);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.TotalCount);
        Assert.Single(result.Data.Items);
        Assert.Equal("SampleJob", result.Data.Items[0].Name);
    }
}
