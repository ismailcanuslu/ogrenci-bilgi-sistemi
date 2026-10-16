using BuildingBlocks.Application.Results;
using Webhooks.Application.Abstractions;
using Webhooks.Domain;

namespace Webhooks.Application.Features;

public static class GetWebhookSubscriptionsFeature
{
    public static async Task<Result<IReadOnlyList<WebhookSubscriptionDto>>> HandleAsync(
        IWebhookRepository repository,
        CancellationToken ct = default)
    {
        var subscriptions = await repository.GetAllSubscriptionsAsync(ct).ConfigureAwait(false);

        var dtos = subscriptions.Select(s => new WebhookSubscriptionDto(
            s.Id.Value,
            s.TargetUrl,
            s.Description,
            s.EventTypes,
            s.IsActive,
            s.CreatedAtUtc,
            s.SecretRotatedAtUtc,
            null)).ToList();

        return Result<IReadOnlyList<WebhookSubscriptionDto>>.Success(dtos);
    }
}
