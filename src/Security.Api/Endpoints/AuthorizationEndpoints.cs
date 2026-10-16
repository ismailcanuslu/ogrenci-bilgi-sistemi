using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using BuildingBlocks.Domain.Authorization;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Security.Application.Features.Authorization;

namespace Security.Api.Endpoints;

public sealed record CreateRoleRequest(string Name, string? Description, string[] Permissions);

public sealed record AssignRoleRequest(Guid RoleId, ScopeType ScopeType, Guid? ScopeId);

public sealed record RevokeAssignmentRequest(string? Reason);

public sealed record CreatePatRequest(
    string Name,
    string[] Permissions,
    ScopeType ScopeType = ScopeType.University,
    Guid? ScopeId = null,
    DateTime? ExpiresAtUtc = null);

public static class AuthorizationEndpoints
{
    public static IEndpointRouteBuilder MapAuthorizationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("authorization", 1, 0)
            .WithTags("Authorization")
            .RequireAuthorization();

        group.MapPost("/roles", async (
            CreateRoleRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(
                new CreateRole.Command(body.Name, body.Description, body.Permissions ?? []), ct)).ToHttpResult(http))
            .WithName("CreateRole")
            .WithSummary("Rol oluşturur (authorization.role.manage)");

        group.MapPost("/users/{userId:guid}/roles", async (
            Guid userId, AssignRoleRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(
                new AssignRole.Command(userId, body.RoleId, body.ScopeType, body.ScopeId), ct)).ToHttpResult(http))
            .WithName("AssignRole")
            .WithSummary("Kullanıcıya kapsamlı rol atar (authorization.role.manage)");

        group.MapDelete("/assignments/{assignmentId:guid}", async (
            Guid assignmentId, [Microsoft.AspNetCore.Mvc.FromBody] RevokeAssignmentRequest? body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(
                new RevokeRoleAssignment.Command(assignmentId, body?.Reason), ct)).ToHttpResult(http))
            .WithName("RevokeRoleAssignment")
            .WithSummary("Rol atamasını iptal eder; geçmiş kayıt korunur");

        group.MapPost("/pats", async (
            CreatePatRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(
                new Security.Application.Features.PersonalAccessTokens.CreatePatCommand(
                    body.Name,
                    body.Permissions ?? [],
                    body.ScopeType,
                    body.ScopeId,
                    body.ExpiresAtUtc), ct)).ToHttpResult(http))
            .WithName("CreatePat")
            .WithSummary("Dış entegrasyonlar için PAT (Personal Access Token) üretir (authorization.pat.create)");

        group.MapGet("/pats", async (
            [AsParameters] Security.Application.Features.PersonalAccessTokens.ListPatsQuery query,
            ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(query, ct)).ToHttpResult(http))
            .WithName("ListPats")
            .WithSummary("PAT token'ları listeler (authorization.pat.read)");

        group.MapDelete("/pats/{id:guid}", async (
            Guid id, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(
                new Security.Application.Features.PersonalAccessTokens.RevokePatCommand(id), ct)).ToHttpResult(http))
            .WithName("RevokePat")
            .WithSummary("PAT token'ı iptal eder (authorization.pat.revoke)");

        return app;
    }
}
