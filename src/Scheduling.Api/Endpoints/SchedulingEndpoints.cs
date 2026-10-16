using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Scheduling.Application.Features;
using Scheduling.Domain;

namespace Scheduling.Api.Endpoints;

public static class SchedulingEndpoints
{
    public static IEndpointRouteBuilder MapSchedulingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("jobs", 1, 0)
            .WithTags("Scheduling");

        group.MapGet("/", async (
            int? page,
            int? pageSize,
            JobStatus? status,
            ISender mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var query = new GetScheduledJobs.Query(page ?? 1, pageSize ?? 20, status);
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult(httpContext);
        })
        .WithName("GetScheduledJobs")
        .WithSummary("Zamanlanmış arka plan işlerini sayfalı ve duruma göre listeleme");

        group.MapPost("/", async (
            ScheduleJobFeature.Command command,
            ISender mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult(httpContext);
        })
        .WithName("ScheduleJob")
        .WithSummary("Yeni bir zamanlanmış veya gecikmeli arka plan işi oluşturma");

        group.MapPost("/{id:guid}/trigger", async (
            Guid id,
            ISender mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var command = new TriggerJobFeature.Command(id);
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult(httpContext);
        })
        .WithName("TriggerJobNow")
        .WithSummary("Belirli bir zamanlanmış işi beklemeden anında çalıştırma");

        group.MapPost("/{id:guid}/cancel", async (
            Guid id,
            ISender mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var command = new CancelJobFeature.Command(id);
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult(httpContext);
        })
        .WithName("CancelJob")
        .WithSummary("Zamanlanmış bir işi iptal etme");

        return app;
    }
}
