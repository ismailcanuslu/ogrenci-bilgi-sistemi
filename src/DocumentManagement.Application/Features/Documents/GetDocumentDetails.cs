using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using DocumentManagement.Application.Abstractions;
using MediatR;

namespace DocumentManagement.Application.Features.Documents;

public sealed record GetDocumentDetailsQuery(Guid DocumentId) : IRequest<ServiceResult<DocumentDetailsDto>>;

public sealed record WorkflowStepDto(
    Guid Id,
    int StepOrder,
    string StepType,
    Guid AssigneeUserId,
    string Status,
    string? Note,
    DateTime? ActionDateUtc);

public sealed record DocumentDetailsDto(
    Guid Id,
    string? DocumentNumber,
    string Subject,
    string ContentSummary,
    string DocumentType,
    string Status,
    Guid CreatorUserId,
    string? DepartmentCode,
    string VerificationCode,
    string VerificationHash,
    string? AttachmentUrl,
    DateTime CreatedAtUtc,
    DateTime? SignedAtUtc,
    IReadOnlyCollection<WorkflowStepDto> WorkflowSteps);

public sealed class GetDocumentDetailsHandler(
    IOfficialDocumentRepository documentRepository,
    IPermissionGuard permissionGuard) : IRequestHandler<GetDocumentDetailsQuery, ServiceResult<DocumentDetailsDto>>
{
    public async Task<ServiceResult<DocumentDetailsDto>> Handle(
        GetDocumentDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var authError = await permissionGuard.CheckAsync(
            Permissions.DocumentManagement.DocumentManage,
            [ResourceScope.University],
            cancellationToken);

        if (authError is not null)
        {
            return AuthorizationFailure.To<DocumentDetailsDto>(authError);
        }

        var document = await documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
        if (document is null)
        {
            return ServiceResult<DocumentDetailsDto>.NotFound(
                new Error("OfficialDocument.NotFound", ErrorType.NotFound));
        }

        var steps = document.WorkflowSteps.Select(s => new WorkflowStepDto(
            s.Id,
            s.StepOrder,
            s.StepType.ToString(),
            s.AssigneeUserId,
            s.Status.ToString(),
            s.Note,
            s.ActionDateUtc
        )).ToList();

        var dto = new DocumentDetailsDto(
            document.Id,
            document.DocumentNumber,
            document.Subject,
            document.ContentSummary,
            document.Type.ToString(),
            document.Status.ToString(),
            document.CreatorUserId,
            document.DepartmentCode,
            document.VerificationCode,
            document.VerificationHash,
            document.AttachmentUrl,
            document.CreatedAtUtc,
            document.SignedAtUtc,
            steps);

        return ServiceResult<DocumentDetailsDto>.Ok(dto);
    }
}
