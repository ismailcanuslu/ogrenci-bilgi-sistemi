using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using StudentApplications.Application.Features.Applications;
using StudentApplications.Domain.Applications;

namespace StudentApplications.Api.Endpoints;

public sealed record SubmitApplicationRequest(
    ApplicationType Type,
    Guid? TargetDepartmentId,
    string? TargetInstitution,
    string Details,
    string? AttachmentUrl,
    decimal? Gpa);

public sealed record ReviewApplicationRequest(
    bool Approve,
    string? Notes = null);

public static class StudentApplicationsEndpoints
{
    public static IEndpointRouteBuilder MapStudentApplicationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("applications", 1, 0)
            .WithTags("StudentApplications (ÇAP, Yandal, Yaz Okulu, Belge Talebi)")
            .RequireAuthorization();

        group.MapPost("/", async (
            SubmitApplicationRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new SubmitApplicationCommand(
                body.Type,
                body.TargetDepartmentId,
                body.TargetInstitution,
                body.Details,
                body.AttachmentUrl,
                body.Gpa), ct)).ToHttpResult(http))
            .WithName("SubmitStudentApplication")
            .WithSummary("ÇAP, Yandal, Yatay Geçiş, Yaz Okulu veya Belge Talebi başvurusu yapar");

        group.MapGet("/", async (
            ApplicationType? type, ApplicationState? state, Guid? targetDepartmentId, bool? onlyMine, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new ListApplicationsQuery(type, state, targetDepartmentId, onlyMine ?? true), ct)).ToHttpResult(http))
            .WithName("ListStudentApplications")
            .WithSummary("Öğrenci başvurularını listeler");

        group.MapPut("/{applicationId:guid}/review", async (
            Guid applicationId, ReviewApplicationRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new ReviewApplicationCommand(applicationId, body.Approve, body.Notes), ct)).ToHttpResult(http))
            .WithName("ReviewStudentApplication")
            .WithSummary("Öğrenci başvurusunu onaylar veya gerekçe ile reddeder");

        return app;
    }
}
