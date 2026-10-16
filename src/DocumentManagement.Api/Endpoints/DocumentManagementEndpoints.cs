using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using DocumentManagement.Application.Features.Documents;
using DocumentManagement.Domain.Documents;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace DocumentManagement.Api.Endpoints;

public sealed record CreateOfficialDocumentRequest(
    string Subject,
    string ContentSummary,
    DocumentType Type,
    string? DepartmentCode,
    string? AttachmentUrl,
    IReadOnlyCollection<WorkflowStepInput> WorkflowSteps,
    bool SubmitImmediately = true);

public sealed record ProcessDocumentStepRequest(
    bool Approve,
    string? Note = null);

public static class DocumentManagementEndpoints
{
    public static IEndpointRouteBuilder MapDocumentManagementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("documents", 1, 0)
            .WithTags("DocumentManagement (EBYS)");

        // 1. Anonim / Harici Belge Doğrulama (Karekod veya Doğrulama Kodu ile)
        group.MapGet("/verify/{verificationCode}", async (
            string verificationCode, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new VerifyDocumentQuery(verificationCode), ct)).ToHttpResult(http))
            .AllowAnonymous()
            .WithName("VerifyDocument")
            .WithSummary("Karekod veya doğrulama kodu ile resmi evrakın geçerliliğini anonim doğrular");

        // 2. Yetkili / Oturumlu EBYS Süreçleri
        var authorizedGroup = group.MapGroup("/")
            .RequireAuthorization();

        authorizedGroup.MapPost("/", async (
            CreateOfficialDocumentRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new CreateOfficialDocumentCommand(
                body.Subject,
                body.ContentSummary,
                body.Type,
                body.DepartmentCode,
                body.AttachmentUrl,
                body.WorkflowSteps,
                body.SubmitImmediately), ct)).ToHttpResult(http))
            .WithName("CreateOfficialDocument")
            .WithSummary("Resmi yazı/evrak oluşturur ve paraf/imza akışını başlatır");

        authorizedGroup.MapGet("/", async (
            DocumentType? type, DocumentStatus? status, bool? onlyMine, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new ListDocumentsQuery(type, status, onlyMine ?? false), ct)).ToHttpResult(http))
            .WithName("ListOfficialDocuments")
            .WithSummary("Resmi yazıları ve evrakları listeler");

        authorizedGroup.MapGet("/{documentId:guid}", async (
            Guid documentId, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new GetDocumentDetailsQuery(documentId), ct)).ToHttpResult(http))
            .WithName("GetOfficialDocumentDetails")
            .WithSummary("Resmi evrakın detaylarını ve paraf/imza akış durumunu görüntüler");

        authorizedGroup.MapPost("/{documentId:guid}/steps/{stepId:guid}/process", async (
            Guid documentId, Guid stepId, ProcessDocumentStepRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new ProcessDocumentStepCommand(documentId, stepId, body.Approve, body.Note), ct)).ToHttpResult(http))
            .WithName("ProcessDocumentWorkflowStep")
            .WithSummary("Evrakın paraf veya imza adımını onaylar veya reddeder");

        return app;
    }
}
