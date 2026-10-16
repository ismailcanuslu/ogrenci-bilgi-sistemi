using System.Security.Claims;
using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using Files.Application.Features;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Files.Api.Endpoints;

public static class FileEndpoints
{
    public static IEndpointRouteBuilder MapFilesEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("files", 1, 0)
            .WithTags("Files");

        group.MapPost("/upload", async (
            IFormFile? file,
            ClaimsPrincipal user,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var fileName = file?.FileName ?? string.Empty;
            var contentType = file?.ContentType ?? string.Empty;
            var fileSize = file?.Length ?? 0;
            var stream = file?.OpenReadStream() ?? Stream.Null;
            var userId = GetUserId(user);

            await using (stream)
            {
                var command = new UploadFile.Command(
                    fileName,
                    contentType,
                    fileSize,
                    stream,
                    userId);

                var result = await mediator.Send(command, cancellationToken);
                return result.ToHttpResult();
            }
        })
        .DisableAntiforgery()
        .WithName("UploadFile")
        .WithSummary("Dosya yükleme");

        group.MapGet("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetFileMetadata.Query(id, GetUserId(user));
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("GetFileMetadata")
        .WithSummary("Dosya metadata bilgisi getirme");

        group.MapGet("/{id:guid}/download", async (
            Guid id,
            ClaimsPrincipal user,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new DownloadFile.Query(id, GetUserId(user));
            var result = await mediator.Send(query, cancellationToken);

            if (!result.IsSuccess || result.Data is null)
            {
                return result.ToHttpResult();
            }

            return TypedResults.Stream(
                result.Data.ContentStream,
                contentType: result.Data.ContentType,
                fileDownloadName: result.Data.FileName);
        })
        .WithName("DownloadFile")
        .WithSummary("Dosya içeriğini indirme");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            ISender mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteFile.Command(id, GetUserId(user));
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("DeleteFile")
        .WithSummary("Dosya silme");

        return app;
    }

    private static Guid? GetUserId(ClaimsPrincipal? principal)
    {
        if (principal is null)
        {
            return null;
        }

        var claimValue = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? principal.FindFirst("sub")?.Value;

        return Guid.TryParse(claimValue, out var id) ? id : null;
    }
}
