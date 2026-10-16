using System.Diagnostics;
using System.Text.Json;
using Messaging.Application.Abstractions;
using Messaging.Domain;

namespace Messaging.Infrastructure.Services;

public sealed class EventPublisher(IOutboxRepository outboxRepository) : IEventPublisher
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
        where T : class, IIntegrationEvent
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);

        var type = integrationEvent.GetType().AssemblyQualifiedName ?? integrationEvent.GetType().FullName!;
        var payloadJson = JsonSerializer.Serialize(integrationEvent, JsonOptions);

        var activity = Activity.Current;
        var traceId = integrationEvent.TraceId ?? activity?.TraceId.ToString();
        var spanId = activity?.SpanId.ToString();

        var message = OutboxMessage.Create(type, payloadJson, traceId, spanId);

        await outboxRepository.AddAsync(message, cancellationToken);

        activity?.AddEvent(new ActivityEvent("OutboxMessageEnqueued", tags: new ActivityTagsCollection
        {
            ["messaging.outbox.id"] = message.Id.Value.ToString(),
            ["messaging.outbox.type"] = type
        }));
    }
}
