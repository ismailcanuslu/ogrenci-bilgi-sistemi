using System.Diagnostics;
using Auditing.Application.Abstractions;
using Auditing.Application.Features;
using Auditing.Domain;
using Auditing.Infrastructure.Services;
using NSubstitute;
using Xunit;

namespace UniversalSystem.UnitTests.Auditing;

public class AuditingTests
{
    [Fact]
    public void AuditLog_Create_ShouldInitializeCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        var log = AuditLog.Create(
            userId: userId,
            userName: "testuser@example.com",
            action: "UserLogin",
            module: "Security",
            resource: "UserSession",
            resourceId: "session-123",
            oldValuesJson: null,
            newValuesJson: "{\"ip\":\"127.0.0.1\"}",
            ipAddress: "127.0.0.1",
            userAgent: "Mozilla/5.0",
            traceId: "4bf92f3577b34da6a3ce929d0e0e4736",
            spanId: "00f067aa0ba902b7",
            timestamp: timestamp,
            isSuccess: true);

        // Assert
        Assert.NotNull(log);
        Assert.NotEqual(Guid.Empty, log.Id.Value);
        Assert.Equal(userId, log.UserId);
        Assert.Equal("UserLogin", log.Action);
        Assert.Equal("Security", log.Module);
        Assert.Equal("4bf92f3577b34da6a3ce929d0e0e4736", log.TraceId);
        Assert.Equal("00f067aa0ba902b7", log.SpanId);
        Assert.True(log.IsSuccess);
    }

    [Fact]
    public async Task AuditService_ShouldRecordLog_WithActiveTelemetryActivity()
    {
        // Arrange
        var repository = Substitute.For<IAuditLogRepository>();
        var service = new AuditService(repository);

        using var activitySource = new ActivitySource("OBS.TestAuditingSource");
        using var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
        };
        ActivitySource.AddActivityListener(listener);

        using var activity = activitySource.StartActivity("TestAuditingActivity");
        Assert.NotNull(activity);

        // Act
        await service.RecordAsync(
            action: "FileUpload",
            module: "Files",
            resource: "FileObject",
            resourceId: "file-999",
            newValues: new { size = 1024 });

        // Assert
        await repository.Received(1).AddAsync(
            Arg.Is<AuditLog>(l =>
                l.Action == "FileUpload" &&
                l.Module == "Files" &&
                l.TraceId == activity.TraceId.ToString()),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAuditLogsHistory_Handler_ShouldReturnPagedResults()
    {
        // Arrange
        var repository = Substitute.For<IAuditLogRepository>();
        var sampleLog = AuditLog.Create(
            userId: Guid.NewGuid(),
            userName: "admin",
            action: "DeleteFile",
            module: "Files",
            resource: "File",
            resourceId: "123",
            oldValuesJson: null,
            newValuesJson: null,
            ipAddress: null,
            userAgent: null,
            traceId: "trace-abc",
            spanId: "span-def",
            timestamp: DateTimeOffset.UtcNow);

        repository.GetPagedAsync(1, 10, null, null, null, null, null, null, Arg.Any<CancellationToken>())
            .Returns((new List<AuditLog> { sampleLog }, 1));

        var handler = new GetAuditLogsHistory.Handler(repository);
        var query = new GetAuditLogsHistory.Query(1, 10);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.TotalCount);
        Assert.Single(result.Data.Items);
        Assert.Equal("DeleteFile", result.Data.Items[0].Action);
        Assert.Equal("trace-abc", result.Data.Items[0].TraceId);
    }
}
