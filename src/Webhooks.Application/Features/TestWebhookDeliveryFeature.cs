using System.Text.Json;
using BuildingBlocks.Application.Results;
using Webhooks.Application.Abstractions;
using Webhooks.Domain;

namespace Webhooks.Application.Features;

public sealed record TestWebhookDeliveryRequest(
    string EventType,
    object? SamplePayload = null);

public static class TestWebhookDeliveryFeature
{
    public static async Task<Result<WebhookDeliveryResult>> HandleAsync(
        Guid subscriptionId,
        TestWebhookDeliveryRequest request,
        IWebhookRepository repository,
        IWebhookDispatcher dispatcher,
        CancellationToken ct = default)
    {
        var subscription = await repository.GetByIdAsync(WebhookSubscriptionId.From(subscriptionId), ct).ConfigureAwait(false);
        if (subscription == null)
        {
            return Result<WebhookDeliveryResult>.Failure(WebhookErrors.SubscriptionNotFound);
        }

        var payloadJson = request.SamplePayload != null
            ? JsonSerializer.Serialize(request.SamplePayload)
            : JsonSerializer.Serialize(new
            {
                Event = request.EventType,
                Test = true,
                Timestamp = DateTime.UtcNow
            });

        var result = await dispatcher.DispatchAsync(subscription, request.EventType, payloadJson, ct).ConfigureAwait(false);
        return Result<WebhookDeliveryResult>.Success(result);
    }
}
