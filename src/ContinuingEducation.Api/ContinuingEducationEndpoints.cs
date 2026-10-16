using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using ContinuingEducation.Application.Features.Programs;
using ContinuingEducation.Domain.Programs;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ContinuingEducation.Api.Endpoints;

public sealed record CreateEducationProgramRequest(
    string Title,
    string Code,
    string? Description,
    Guid InstructorUserId,
    int Capacity,
    DateTime StartDateUtc,
    DateTime EndDateUtc,
    int TotalHours,
    decimal Price,
    bool IsFree,
    int MinAttendancePercentage = 70,
    decimal MinPassingGrade = 60);

public sealed record ReviewProgramApplicationRequest(
    bool Approve,
    string? Reason = null);

public sealed record RecordAttendanceRequest(
    int AttendedHours);

public sealed record EvaluateApplicationRequest(
    decimal FinalGrade);

public static class ContinuingEducationEndpoints
{
    public static IEndpointRouteBuilder MapContinuingEducationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("continuing-education", 1, 0)
            .WithTags("ContinuingEducation")
            .RequireAuthorization();

        MapPrograms(group);
        MapApplications(group);

        return app;
    }

    private static void MapPrograms(RouteGroupBuilder group)
    {
        var programs = group.MapGroup("/programs");

        programs.MapPost("/", async (
            CreateEducationProgramRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new CreateEducationProgramCommand(
                body.Title,
                body.Code,
                body.Description,
                body.InstructorUserId,
                body.Capacity,
                body.StartDateUtc,
                body.EndDateUtc,
                body.TotalHours,
                body.Price,
                body.IsFree,
                body.MinAttendancePercentage,
                body.MinPassingGrade), ct)).ToHttpResult(http))
            .WithName("CreateEducationProgram")
            .WithSummary("Sürekli Eğitim Merkezi (SEM) kurs veya eğitim programı oluşturur");

        programs.MapGet("/", async (
            CourseProgramStatus? status, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new ListEducationProgramsQuery(status), ct)).ToHttpResult(http))
            .WithName("ListEducationPrograms")
            .WithSummary("SEM kurs ve eğitim programlarını listeler");

        programs.MapPost("/{programId:guid}/apply", async (
            Guid programId, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new ApplyToEducationProgramCommand(programId), ct)).ToHttpResult(http))
            .WithName("ApplyToEducationProgram")
            .WithSummary("SEM kurs veya eğitim programına başvuru yapar");
    }

    private static void MapApplications(RouteGroupBuilder group)
    {
        var applications = group.MapGroup("/applications");

        applications.MapGet("/", async (
            Guid? programId, bool? onlyMine, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new ListProgramApplicationsQuery(programId, onlyMine ?? true), ct)).ToHttpResult(http))
            .WithName("ListProgramApplications")
            .WithSummary("SEM program başvurularını listeler");

        applications.MapPut("/{applicationId:guid}/review", async (
            Guid applicationId, ReviewProgramApplicationRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new ReviewProgramApplicationCommand(applicationId, body.Approve, body.Reason), ct)).ToHttpResult(http))
            .WithName("ReviewProgramApplication")
            .WithSummary("SEM başvurusunu onaylar veya reddeder");

        applications.MapPost("/{applicationId:guid}/attendance", async (
            Guid applicationId, RecordAttendanceRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new RecordProgramAttendanceCommand(applicationId, body.AttendedHours), ct)).ToHttpResult(http))
            .WithName("RecordProgramAttendance")
            .WithSummary("SEM kursiyerine devamsızlık/yoklama saati işler");

        applications.MapPost("/{applicationId:guid}/evaluate", async (
            Guid applicationId, EvaluateApplicationRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new EvaluateProgramApplicationCommand(applicationId, body.FinalGrade), ct)).ToHttpResult(http))
            .WithName("EvaluateProgramApplication")
            .WithSummary("SEM kursiyerini notlandırır ve devam koşuluyla sertifika durumunu neticelendirir");
    }
}
