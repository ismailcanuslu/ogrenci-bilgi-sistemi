using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using BuildingBlocks.Application.Abstractions.Time;
using ImportExport.Application.Abstractions;
using ImportExport.Application.Features;
using ImportExport.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ImportExport.Api.Endpoints;

public static class ImportExportEndpoints
{
    public static IEndpointRouteBuilder MapImportExportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("import-export", 1, 0)
            .WithTags("ImportExport");

        group.MapPost("/jobs", async (
            CreateImportJobRequest request,
            IImportJobRepository repository,
            IDateTimeProvider dateTimeProvider,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await CreateImportJobFeature.HandleAsync(request, repository, dateTimeProvider, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithName("CreateImportJob")
        .WithSummary("Yeni bir içe aktarma (Import) işi tanımlar.");

        group.MapGet("/jobs", async (
            IImportJobRepository repository,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await GetImportJobsFeature.HandleAsync(repository, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithName("GetImportJobs")
        .WithSummary("Tüm içe aktarma işlerini listeler.");

        return app;
    }
}
