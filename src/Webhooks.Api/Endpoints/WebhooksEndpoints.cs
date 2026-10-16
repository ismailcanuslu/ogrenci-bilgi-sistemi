using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using BuildingBlocks.Application.Abstractions.Time;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Webhooks.Application.Abstractions;
using Webhooks.Application.Features;

namespace Webhooks.Api.Endpoints;

public static class WebhooksEndpoints
{
    public static IEndpointRouteBuilder MapWebhooksEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("webhooks", 1, 0)
            .WithTags("Webhooks");

        group.MapPost("/subscriptions", async (
            CreateWebhookSubscriptionRequest request,
            IWebhookRepository repository,
            IDateTimeProvider dateTimeProvider,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await CreateWebhookSubscriptionFeature.HandleAsync(request, repository, dateTimeProvider, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithName("CreateWebhookSubscription")
        .WithSummary("Yeni bir webhook aboneliği oluşturur ve imzalama için secret üretir.");

        group.MapGet("/subscriptions", async (
            IWebhookRepository repository,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await GetWebhookSubscriptionsFeature.HandleAsync(repository, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithName("GetWebhookSubscriptions")
        .WithSummary("Tüm webhook aboneliklerini listeler.");

        group.MapPost("/subscriptions/{id:guid}/rotate-secret", async (
            Guid id,
            IWebhookRepository repository,
            IDateTimeProvider dateTimeProvider,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await RotateWebhookSecretFeature.HandleAsync(id, repository, dateTimeProvider, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithName("RotateWebhookSecret")
        .WithSummary("Webhook abonesinin imzalama anahtarını güvenli şekilde yeniler (Secret rotation).");

        group.MapPost("/subscriptions/{id:guid}/test", async (
            Guid id,
            TestWebhookDeliveryRequest request,
            IWebhookRepository repository,
            IWebhookDispatcher dispatcher,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await TestWebhookDeliveryFeature.HandleAsync(id, request, repository, dispatcher, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithName("TestWebhookDelivery")
        .WithSummary("Belirtilen webhook abonesine test teslimatı gönderir.");

        group.MapGet("/logs", async (
            Guid? subscriptionId,
            int? limit,
            IWebhookRepository repository,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await GetWebhookDeliveryLogsFeature.HandleAsync(subscriptionId, limit ?? 50, repository, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithName("GetWebhookDeliveryLogs")
        .WithSummary("Webhook teslimat geçmişini (başarı, HTTP kodu, deneme sayısı, hata) sorgular.");

        return app;
    }
}
