using System.Diagnostics;
using System.Text.Json;
using Messaging.Application.Abstractions;
using Messaging.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messaging.Infrastructure.Background;

public sealed class OutboxProcessorBackgroundService(
    IServiceScopeFactory scopeFactory,
    IMessageBus messageBus,
    ILogger<OutboxProcessorBackgroundService> logger)
    : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new("OBS.Messaging.OutboxProcessor", "1.0.0");
    private const int BatchSize = 20;
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Outbox Processor Background Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred during outbox processing cycle.");
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

        logger.LogInformation("Outbox Processor Background Service stopped.");
    }

    public async Task<int> ProcessBatchAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();

        var batch = await repository.GetUnprocessedBatchAsync(BatchSize, cancellationToken);
        if (batch.Count == 0)
        {
            return 0;
        }

        logger.LogDebug("Fetched {Count} unprocessed outbox messages.", batch.Count);

        foreach (var message in batch)
        {
            ActivityContext parentContext = default;
            if (!string.IsNullOrWhiteSpace(message.TraceId) && message.TraceId.Length == 32)
            {
                try
                {
                    var traceId = ActivityTraceId.CreateFromString(message.TraceId.AsSpan());
                    var spanId = !string.IsNullOrWhiteSpace(message.SpanId) && message.SpanId.Length == 16
                        ? ActivitySpanId.CreateFromString(message.SpanId.AsSpan())
                        : ActivitySpanId.CreateRandom();

                    parentContext = new ActivityContext(traceId, spanId, ActivityTraceFlags.Recorded);
                }
                catch
                {
                    // fallback to default parentContext
                }
            }

            using var activity = ActivitySource.StartActivity(
                "ProcessOutboxMessage",
                ActivityKind.Consumer,
                parentContext);

            activity?.SetTag("messaging.outbox.id", message.Id.Value.ToString());
            activity?.SetTag("messaging.outbox.type", message.Type);

            try
            {
                var eventType = Type.GetType(message.Type);
                object? payload = null;

                if (eventType is not null)
                {
                    payload = JsonSerializer.Deserialize(message.PayloadJson, eventType);
                }

                if (payload is not null)
                {
                    await messageBus.PublishAsync(payload, cancellationToken);
                }
                else
                {
                    logger.LogWarning("Could not resolve type {Type} for outbox message {Id}. Marking as processed generic.", message.Type, message.Id.Value);
                }

                message.MarkAsProcessed(DateTimeOffset.UtcNow);
                activity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process outbox message {Id} of type {Type}.", message.Id.Value, message.Type);
                message.RecordFailure(ex.Message);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            }

            await repository.UpdateAsync(message, cancellationToken);
        }

        return batch.Count;
    }
}
