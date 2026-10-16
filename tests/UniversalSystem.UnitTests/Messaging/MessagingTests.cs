using System.Diagnostics;
using Messaging.Application.Abstractions;
using Messaging.Application.Features;
using Messaging.Domain;
using Messaging.Infrastructure.Bus;
using Messaging.Infrastructure.Services;
using NSubstitute;
using Xunit;

namespace UniversalSystem.UnitTests.Messaging;

public class MessagingTests
{
    private sealed record UserRegisteredIntegrationEvent(
        Guid Id,
        DateTimeOffset OccurredOnUtc,
        Guid UserId,
        string Email,
        string? TraceId) : IIntegrationEvent;

    [Fact]
    public void OutboxMessage_Create_And_Lifecycle_ShouldWork()
    {
        // Arrange
        var message = OutboxMessage.Create(
            type: "UserRegisteredIntegrationEvent",
            payloadJson: "{\"userId\":\"11111111-1111-1111-1111-111111111111\"}",
            traceId: "trace-xyz",
            spanId: "span-xyz");

        // Assert initial
        Assert.NotNull(message);
        Assert.False(message.IsProcessed);
        Assert.Equal(0, message.RetryCount);
        Assert.Null(message.ProcessedOnUtc);
        Assert.Equal("trace-xyz", message.TraceId);

        // Act - record failure
        message.RecordFailure("Connection timeout");
        Assert.Equal(1, message.RetryCount);
        Assert.Equal("Connection timeout", message.Error);
        Assert.False(message.IsProcessed);

        // Act - mark processed
        var now = DateTimeOffset.UtcNow;
        message.MarkAsProcessed(now);
        Assert.True(message.IsProcessed);
        Assert.Equal(now, message.ProcessedOnUtc);
        Assert.Null(message.Error);
    }

    [Fact]
    public async Task ChannelMessageBus_PublishAndSubscribe_ShouldDeliverMessage()
    {
        // Arrange
        var bus = new ChannelMessageBus();
        UserRegisteredIntegrationEvent? receivedEvent = null;

        using var subscription = bus.Subscribe<UserRegisteredIntegrationEvent>((evt, _) =>
        {
            receivedEvent = evt;
            return Task.CompletedTask;
        });

        var sentEvent = new UserRegisteredIntegrationEvent(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            Guid.NewGuid(),
            "test@example.com",
            "trace-123");

        // Act
        await bus.PublishAsync(sentEvent);

        // Assert
        Assert.NotNull(receivedEvent);
        Assert.Equal(sentEvent.Id, receivedEvent.Id);
        Assert.Equal("test@example.com", receivedEvent.Email);
    }

    [Fact]
    public async Task EventPublisher_ShouldAddOutboxMessage_WithTraceId()
    {
        // Arrange
        var repository = Substitute.For<IOutboxRepository>();
        var publisher = new EventPublisher(repository);

        var integrationEvent = new UserRegisteredIntegrationEvent(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            Guid.NewGuid(),
            "welcome@example.com",
            "trace-otel-abc");

        // Act
        await publisher.PublishAsync(integrationEvent);

        // Assert
        await repository.Received(1).AddAsync(
            Arg.Is<OutboxMessage>(m =>
                m.TraceId == "trace-otel-abc" &&
                m.PayloadJson.Contains("welcome@example.com")),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOutboxMessages_Handler_ShouldReturnPagedResults()
    {
        // Arrange
        var repository = Substitute.For<IOutboxRepository>();
        var sampleMessage = OutboxMessage.Create(
            type: "SampleEvent",
            payloadJson: "{}",
            traceId: "trace-1");

        repository.GetPagedAsync(1, 10, null, Arg.Any<CancellationToken>())
            .Returns((new List<OutboxMessage> { sampleMessage }, 1));

        var handler = new GetOutboxMessages.Handler(repository);
        var query = new GetOutboxMessages.Query(1, 10);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.TotalCount);
        Assert.Single(result.Data.Items);
        Assert.Equal("SampleEvent", result.Data.Items[0].Type);
    }
}
