using BuildingBlocks.Application.Results;
using Webhooks.Application.Abstractions;
using Webhooks.Domain;

namespace Webhooks.Application.Features;

public sealed record WebhookDeliveryLogDto(
    Guid Id,
    Guid SubscriptionId,
    string EventType,
    string Payload,
    int? StatusCode,
    bool IsSuccess,
    int AttemptCount,
    string? ErrorMessage,
    DateTime DispatchedAtUtc);

public static class GetWebhookDeliveryLogsFeature
{
    public static async Task<Result<IReadOnlyList<WebhookDeliveryLogDto>>> HandleAsync(
        Guid? subscriptionId,
        int limit,
        IWebhookRepository repository,
        CancellationToken ct = default)
    {
        var logs = await repository.GetDeliveryLogsAsync(
            subscriptionId.HasValue ? WebhookSubscriptionId.From(subscriptionId.Value) : null,
            limit <= 0 ? 50 : Math.Min(limit, 200),
            ct).ConfigureAwait(false);

        var dtos = logs.Select(l => new WebhookDeliveryLogDto(
            l.Id.Value,
            l.SubscriptionId.Value,
            l.EventType,
            l.Payload,
            l.StatusCode,
            l.IsSuccess,
            l.AttemptCount,
            l.ErrorMessage,
            l.DispatchedAtUtc)).ToList();

        return Result<IReadOnlyList<WebhookDeliveryLogDto>>.Success(dtos);
    }
}
