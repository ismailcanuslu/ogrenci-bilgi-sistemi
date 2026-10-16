using BuildingBlocks.Application.Results;

namespace Webhooks.Domain;

public static class WebhookErrors
{
    public static readonly Error SubscriptionNotFound =
        new(
            Code: "Webhooks.SubscriptionNotFound",
            Type: ErrorType.NotFound);

    public static readonly Error InvalidUrl =
        new(
            Code: "Webhooks.InvalidUrl",
            Type: ErrorType.Validation);

    public static readonly Error DeliveryFailed =
        new(
            Code: "Webhooks.DeliveryFailed",
            Type: ErrorType.Failure);

    public static readonly Error InvalidSignature =
        new(
            Code: "Webhooks.InvalidSignature",
            Type: ErrorType.Validation);
}
