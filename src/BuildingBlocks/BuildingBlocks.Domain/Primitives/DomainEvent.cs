using BuildingBlocks.Domain.Abstractions;

namespace BuildingBlocks.Domain.Primitives;

public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();

    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}