namespace Messaging.Domain;

public readonly record struct OutboxMessageId(Guid Value)
{
    public static OutboxMessageId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
