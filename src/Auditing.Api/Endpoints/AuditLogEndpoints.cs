using Auditing.Application.Features;
using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Auditing.Api.Endpoints;

public static class AuditLogEndpoints
{
    public static IEndpointRouteBuilder MapAuditLogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("audit-logs", 1, 0)
            .WithTags("Auditing");

        group.MapGet("/", async (
            int? page,
            int? pageSize,
            string? module,
            string? action,
            Guid? userId,
            string? resource,
            DateTimeOffset? from,
            DateTimeOffset? to,
            ISender mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAuditLogsHistory.Query(
                Page: page ?? 1,
                PageSize: pageSize ?? 20,
                Module: module,
                Action: action,
                UserId: userId,
                Resource: resource,
                From: from,
                To: to);

            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult(httpContext);
        })
        .WithName("GetAuditLogsHistory")
        .WithSummary("Sistem denetim ve izleme kayıtlarını filtreli ve sayfalı olarak listeleme");

        group.MapGet("/{id:guid}", async (
            Guid id,
            ISender mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAuditLogById.Query(id);
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult(httpContext);
        })
        .WithName("GetAuditLogById")
        .WithSummary("Belirli bir denetim günlüğü kaydını detaylarıyla getirme");

        return app;
    }
}
