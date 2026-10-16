namespace Webhooks.Domain;

public readonly record struct WebhookSubscriptionId(Guid Value)
{
    public static WebhookSubscriptionId New() => new(Guid.NewGuid());
    public static WebhookSubscriptionId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
