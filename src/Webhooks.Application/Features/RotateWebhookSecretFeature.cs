using BuildingBlocks.Application.Abstractions.Time;
using BuildingBlocks.Application.Results;
using Webhooks.Application.Abstractions;
using Webhooks.Domain;

namespace Webhooks.Application.Features;

public sealed record RotateWebhookSecretResponse(
    Guid Id,
    string NewSecret,
    DateTime SecretRotatedAtUtc);

public static class RotateWebhookSecretFeature
{
    public static async Task<Result<RotateWebhookSecretResponse>> HandleAsync(
        Guid subscriptionId,
        IWebhookRepository repository,
        IDateTimeProvider dateTimeProvider,
        CancellationToken ct = default)
    {
        var subscription = await repository.GetByIdAsync(WebhookSubscriptionId.From(subscriptionId), ct).ConfigureAwait(false);
        if (subscription == null)
        {
            return Result<RotateWebhookSecretResponse>.Failure(WebhookErrors.SubscriptionNotFound);
        }

        subscription.RotateSecret(dateTimeProvider.UtcNow.UtcDateTime);
        await repository.UpdateAsync(subscription, ct).ConfigureAwait(false);

        return Result<RotateWebhookSecretResponse>.Success(new RotateWebhookSecretResponse(
            subscription.Id.Value,
            subscription.Secret,
            subscription.SecretRotatedAtUtc!.Value));
    }
}
