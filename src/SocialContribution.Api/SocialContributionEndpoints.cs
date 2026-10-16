using System.Security.Claims;
using BuildingBlocks.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SocialContribution.Application.Features;
using SocialContribution.Domain;

namespace SocialContribution.Api;

public static class SocialContributionEndpoints
{
    public static IEndpointRouteBuilder MapSocialContributionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/social-contribution")
            .WithTags("Toplumsal Katkı Koordinatörlüğü");

        // Public / Öğrenci & Personel listeleme
        group.MapGet("/projects", async (
            ContributionCategory? category,
            ProjectStatus? status,
            [FromServices] IQueryHandler<GetSocialProjectsQuery, IReadOnlyCollection<SocialContributionProjectDto>> handler,
            CancellationToken ct) =>
        {
            var res = await handler.Handle(new GetSocialProjectsQuery(category, status), ct);
            return Results.Ok(res);
        }).RequireAuthorization();

        group.MapGet("/projects/{id:guid}", async (
            Guid id,
            [FromServices] IQueryHandler<GetSocialProjectByIdQuery, SocialContributionProjectDto?> handler,
            CancellationToken ct) =>
        {
            var res = await handler.Handle(new GetSocialProjectByIdQuery(id), ct);
            return res == null ? Results.NotFound() : Results.Ok(res);
        }).RequireAuthorization();

        // Proje oluşturma
        group.MapPost("/projects", async (
            CreateSocialProjectRequest req,
            ClaimsPrincipal user,
            [FromServices] ICommandHandler<CreateSocialProjectCommand, Guid> handler,
            CancellationToken ct) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
            var fullName = user.FindFirstValue(ClaimTypes.Name) ?? "Koordinatör";
            var userId = Guid.TryParse(userIdStr, out var u) ? u : Guid.NewGuid();

            var id = await handler.Handle(new CreateSocialProjectCommand(
                req.Title,
                req.Description,
                req.Category,
                userId,
                fullName,
                req.TargetAudience,
                req.PartnerOrganization,
                req.StartDate,
                req.EndDate,
                req.VolunteerQuota), ct);

            return Results.Created($"/api/v1/social-contribution/projects/{id}", new { Id = id });
        }).RequireAuthorization();

        // Koordinatörün kendi projeleri
        group.MapGet("/my-projects", async (
            ClaimsPrincipal user,
            [FromServices] IQueryHandler<GetCoordinatorProjectsQuery, IReadOnlyCollection<SocialContributionProjectDto>> handler,
            CancellationToken ct) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
            if (!Guid.TryParse(userIdStr, out var userId)) return Results.Unauthorized();

            var list = await handler.Handle(new GetCoordinatorProjectsQuery(userId), ct);
            return Results.Ok(list);
        }).RequireAuthorization();

        // Gönüllünün başvurduğu projeler
        group.MapGet("/my-volunteering", async (
            ClaimsPrincipal user,
            [FromServices] IQueryHandler<GetUserVolunteeredProjectsQuery, IReadOnlyCollection<SocialContributionProjectDto>> handler,
            CancellationToken ct) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
            if (!Guid.TryParse(userIdStr, out var userId)) return Results.Unauthorized();

            var list = await handler.Handle(new GetUserVolunteeredProjectsQuery(userId), ct);
            return Results.Ok(list);
        }).RequireAuthorization();

        // Gönüllü başvurusu yapma
        group.MapPost("/projects/{projectId:guid}/apply-volunteer", async (
            Guid projectId,
            ApplyVolunteerRequest req,
            ClaimsPrincipal user,
            [FromServices] ICommandHandler<ApplyForVolunteeringCommand, Guid> handler,
            CancellationToken ct) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
            if (!Guid.TryParse(userIdStr, out var userId)) return Results.Unauthorized();

            var volId = await handler.Handle(new ApplyForVolunteeringCommand(
                projectId,
                userId,
                req.StudentOrStaffNo,
                req.FullName,
                req.Email,
                req.Note), ct);

            return Results.Ok(new { VolunteerApplicationId = volId });
        }).RequireAuthorization();

        // Proje gönüllülerini listeleme (Koordinatör veya Yönetici)
        group.MapGet("/projects/{projectId:guid}/volunteers", async (
            Guid projectId,
            [FromServices] IQueryHandler<GetProjectVolunteersQuery, IReadOnlyCollection<VolunteerApplicationDto>> handler,
            CancellationToken ct) =>
        {
            var res = await handler.Handle(new GetProjectVolunteersQuery(projectId), ct);
            return Results.Ok(res);
        }).RequireAuthorization();

        // Gönüllü başvurusunu onaylama / reddetme
        group.MapPut("/projects/{projectId:guid}/volunteers/{volunteerId:guid}/status", async (
            Guid projectId,
            Guid volunteerId,
            EvaluateVolunteerRequest req,
            [FromServices] ICommandHandler<EvaluateVolunteerCommand, bool> handler,
            CancellationToken ct) =>
        {
            var success = await handler.Handle(new EvaluateVolunteerCommand(projectId, volunteerId, req.Status), ct);
            return success ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization();

        // Proje Onay / Ret / Başlat / Tamamla (Koordinatörlük & Yönetici)
        group.MapPut("/projects/{projectId:guid}/approve", async (
            Guid projectId,
            [FromServices] ICommandHandler<ApproveSocialProjectCommand, bool> handler,
            CancellationToken ct) =>
        {
            var success = await handler.Handle(new ApproveSocialProjectCommand(projectId), ct);
            return success ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization();

        group.MapPut("/projects/{projectId:guid}/reject", async (
            Guid projectId,
            RejectProjectRequest req,
            [FromServices] ICommandHandler<RejectSocialProjectCommand, bool> handler,
            CancellationToken ct) =>
        {
            var success = await handler.Handle(new RejectSocialProjectCommand(projectId, req.Reason), ct);
            return success ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization();

        group.MapPut("/projects/{projectId:guid}/start", async (
            Guid projectId,
            [FromServices] ICommandHandler<StartSocialProjectCommand, bool> handler,
            CancellationToken ct) =>
        {
            var success = await handler.Handle(new StartSocialProjectCommand(projectId), ct);
            return success ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization();

        group.MapPut("/projects/{projectId:guid}/complete", async (
            Guid projectId,
            CompleteProjectRequest req,
            [FromServices] ICommandHandler<CompleteSocialProjectCommand, bool> handler,
            CancellationToken ct) =>
        {
            var success = await handler.Handle(new CompleteSocialProjectCommand(projectId, req.ImpactSummary, req.BeneficiaryCount), ct);
            return success ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization();

        return app;
    }
}

public record CreateSocialProjectRequest(
    string Title,
    string Description,
    ContributionCategory Category,
    string TargetAudience,
    string PartnerOrganization,
    DateTime StartDate,
    DateTime EndDate,
    int VolunteerQuota);

public record ApplyVolunteerRequest(
    string StudentOrStaffNo,
    string FullName,
    string Email,
    string Note);

public record EvaluateVolunteerRequest(VolunteerApplicationStatus Status);
public record RejectProjectRequest(string Reason);
public record CompleteProjectRequest(string ImpactSummary, int BeneficiaryCount);
