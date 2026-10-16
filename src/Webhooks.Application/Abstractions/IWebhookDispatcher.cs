using Webhooks.Domain;

namespace Webhooks.Application.Abstractions;

public sealed record WebhookDeliveryResult(
    bool IsSuccess,
    int? StatusCode,
    int AttemptCount,
    string? ErrorMessage,
    DateTime DispatchedAtUtc);

public interface IWebhookDispatcher
{
    /// <summary>
    /// Tek bir webhook abonesine HTTP POST çağrısıyla imzalı yük gönderir, teslimat sonucunu günlüğe kaydeder.
    /// </summary>
    Task<WebhookDeliveryResult> DispatchAsync(WebhookSubscription subscription, string eventType, string payloadJson, CancellationToken ct = default);

    /// <summary>
    /// Belirtilen olaya abone olan tüm aktif abonelere asenkron gönderim yapar.
    /// </summary>
    Task<int> PublishEventAsync(string eventType, object payload, CancellationToken ct = default);
}
