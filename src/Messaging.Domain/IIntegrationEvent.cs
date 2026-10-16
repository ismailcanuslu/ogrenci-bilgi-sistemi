namespace Messaging.Domain;

public interface IIntegrationEvent
{
    Guid Id { get; }

    DateTimeOffset OccurredOnUtc { get; }

    string? TraceId { get; }
}
