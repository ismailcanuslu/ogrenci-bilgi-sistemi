using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using MediatR;
using Messaging.Application.Abstractions;
using Messaging.Application.Features;
using Messaging.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Messaging.Api.Endpoints;

public static class MessagingEndpoints
{
    public sealed record TestEvent(
        Guid Id,
        DateTimeOffset OccurredOnUtc,
        string Name,
        string Payload,
        string? TraceId) : IIntegrationEvent;

    public sealed record TestEventRequest(string Name, string Payload);

    public static IEndpointRouteBuilder MapMessagingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("messaging", 1, 0)
            .WithTags("Messaging");

        group.MapGet("/outbox", async (
            int? page,
            int? pageSize,
            bool? processedOnly,
            ISender mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var query = new GetOutboxMessages.Query(
                Page: page ?? 1,
                PageSize: pageSize ?? 20,
                ProcessedOnly: processedOnly);

            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult(httpContext);
        })
        .WithName("GetOutboxMessages")
        .WithSummary("Outbox mesaj kuyruğunu, işlenme ve hata durumlarını listeleme");

        group.MapPost("/test-event", async (
            TestEventRequest request,
            IEventPublisher publisher,
            CancellationToken cancellationToken) =>
        {
            var testEvent = new TestEvent(
                Id: Guid.NewGuid(),
                OccurredOnUtc: DateTimeOffset.UtcNow,
                Name: request.Name,
                Payload: request.Payload,
                TraceId: System.Diagnostics.Activity.Current?.TraceId.ToString());

            await publisher.PublishAsync(testEvent, cancellationToken);

            return Results.Accepted(
                value: new
                {
                    message = "Test olayı outbox kuyruğuna alındı ve OpenTelemetry izlemesi başlatıldı.",
                    eventId = testEvent.Id,
                    traceId = testEvent.TraceId
                });
        })
        .WithName("PublishTestIntegrationEvent")
        .WithSummary("Test entegrasyon olayı yayınlama ve outbox kuyruğuna yazma");

        return app;
    }
}
