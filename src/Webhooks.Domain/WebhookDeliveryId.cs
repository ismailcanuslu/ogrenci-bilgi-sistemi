namespace Webhooks.Domain;

public readonly record struct WebhookDeliveryId(Guid Value)
{
    public static WebhookDeliveryId New() => new(Guid.NewGuid());
    public static WebhookDeliveryId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
