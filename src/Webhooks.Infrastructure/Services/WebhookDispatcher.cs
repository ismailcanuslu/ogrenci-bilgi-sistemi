using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BuildingBlocks.Application.Abstractions.Time;
using Microsoft.Extensions.Logging;
using Webhooks.Application.Abstractions;
using Webhooks.Domain;

namespace Webhooks.Infrastructure.Services;

public sealed class WebhookDispatcher : IWebhookDispatcher
{
    private static readonly ActivitySource ActivitySource = new("OBS.Webhooks.Dispatcher", "1.0.0");

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWebhookSigner _signer;
    private readonly IWebhookRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<WebhookDispatcher> _logger;

    public WebhookDispatcher(
        IHttpClientFactory httpClientFactory,
        IWebhookSigner signer,
        IWebhookRepository repository,
        IDateTimeProvider dateTimeProvider,
        ILogger<WebhookDispatcher> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _signer = signer ?? throw new ArgumentNullException(nameof(signer));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<WebhookDeliveryResult> DispatchAsync(
        WebhookSubscription subscription,
        string eventType,
        string payloadJson,
        CancellationToken ct = default)
    {
        using var activity = ActivitySource.StartActivity("Webhook.Dispatch", ActivityKind.Client);
        activity?.SetTag("webhook.subscription_id", subscription.Id.Value.ToString());
        activity?.SetTag("webhook.event_type", eventType);
        activity?.SetTag("webhook.target_url", subscription.TargetUrl);

        var deliveryId = WebhookDeliveryId.New();
        var signature = _signer.ComputeSignature(payloadJson, subscription.Secret);
        var client = _httpClientFactory.CreateClient("WebhooksClient");

        const int maxAttempts = 3;
        int currentAttempt = 0;
        int? lastStatusCode = null;
        bool isSuccess = false;
        string? errorMessage = null;

        while (currentAttempt < maxAttempts && !isSuccess)
        {
            currentAttempt++;
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, subscription.TargetUrl);
                request.Content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
                request.Headers.Add("X-Hub-Signature-256", signature);
                request.Headers.Add("X-Webhook-Event", eventType);
                request.Headers.Add("X-Webhook-Delivery", deliveryId.Value.ToString());

                using var response = await client.SendAsync(request, ct).ConfigureAwait(false);
                lastStatusCode = (int)response.StatusCode;
                isSuccess = response.IsSuccessStatusCode;

                if (!isSuccess)
                {
                    errorMessage = $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}";
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                lastStatusCode = null;
                isSuccess = false;
                errorMessage = ex.Message;
                _logger.LogWarning(ex, "Webhook gönderimi denemesi {Attempt} başarısız oldu: {Url}", currentAttempt, subscription.TargetUrl);
            }

            if (!isSuccess && currentAttempt < maxAttempts)
            {
                // Exponential backoff
                await Task.Delay(TimeSpan.FromMilliseconds(200 * Math.Pow(2, currentAttempt - 1)), ct).ConfigureAwait(false);
            }
        }

        var dispatchedAt = _dateTimeProvider.UtcNow.UtcDateTime;
        var log = new WebhookDeliveryLog(
            subscription.Id,
            eventType,
            payloadJson,
            lastStatusCode,
            isSuccess,
            currentAttempt,
            errorMessage,
            dispatchedAt);

        await _repository.AddDeliveryLogAsync(log, ct).ConfigureAwait(false);

        activity?.SetTag("webhook.is_success", isSuccess);
        activity?.SetTag("webhook.status_code", lastStatusCode);
        activity?.SetTag("webhook.attempts", currentAttempt);

        return new WebhookDeliveryResult(
            isSuccess,
            lastStatusCode,
            currentAttempt,
            errorMessage,
            dispatchedAt);
    }

    public async Task<int> PublishEventAsync(string eventType, object payload, CancellationToken ct = default)
    {
        var subscriptions = await _repository.GetActiveSubscriptionsForEventAsync(eventType, ct).ConfigureAwait(false);
        if (subscriptions.Count == 0)
        {
            return 0;
        }

        var payloadJson = JsonSerializer.Serialize(payload);
        int successCount = 0;

        foreach (var sub in subscriptions)
        {
            var result = await DispatchAsync(sub, eventType, payloadJson, ct).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                successCount++;
            }
        }

        return successCount;
    }
}
