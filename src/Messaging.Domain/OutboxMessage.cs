using BuildingBlocks.Domain.Primitives;

namespace Messaging.Domain;

public sealed class OutboxMessage : Entity<OutboxMessageId>
{
    private OutboxMessage(
        OutboxMessageId id,
        DateTimeOffset occurredOnUtc,
        string type,
        string payloadJson,
        string? traceId,
        string? spanId)
        : base(id)
    {
        OccurredOnUtc = occurredOnUtc;
        Type = type;
        PayloadJson = payloadJson;
        TraceId = traceId;
        SpanId = spanId;
        RetryCount = 0;
    }

    private OutboxMessage()
    {
    }

    public DateTimeOffset OccurredOnUtc { get; private set; }

    public string Type { get; private set; } = default!;

    public string PayloadJson { get; private set; } = default!;

    public DateTimeOffset? ProcessedOnUtc { get; private set; }

    public string? Error { get; private set; }

    public int RetryCount { get; private set; }

    public string? TraceId { get; private set; }

    public string? SpanId { get; private set; }

    public bool IsProcessed => ProcessedOnUtc.HasValue;

    public static OutboxMessage Create(
        string type,
        string payloadJson,
        string? traceId = null,
        string? spanId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(payloadJson);

        return new OutboxMessage(
            OutboxMessageId.New(),
            DateTimeOffset.UtcNow,
            type,
            payloadJson,
            traceId,
            spanId);
    }

    public void MarkAsProcessed(DateTimeOffset processedOnUtc)
    {
        ProcessedOnUtc = processedOnUtc;
        Error = null;
    }

    public void RecordFailure(string error)
    {
        RetryCount++;
        Error = error;
    }
}
