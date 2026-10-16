using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using DocumentManagement.Application.Abstractions;
using DocumentManagement.Domain.Documents;
using FluentValidation;
using MediatR;

namespace DocumentManagement.Application.Features.Documents;

public sealed record WorkflowStepInput(WorkflowStepType StepType, Guid AssigneeUserId);

public sealed record CreateOfficialDocumentCommand(
    string Subject,
    string ContentSummary,
    DocumentType Type,
    string? DepartmentCode,
    string? AttachmentUrl,
    IReadOnlyCollection<WorkflowStepInput> WorkflowSteps,
    bool SubmitImmediately = true) : IRequest<ServiceResult<Guid>>;

public sealed class CreateOfficialDocumentValidator : AbstractValidator<CreateOfficialDocumentCommand>
{
    public CreateOfficialDocumentValidator()
    {
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(OfficialDocument.SubjectMaxLength);
        RuleFor(x => x.ContentSummary).NotEmpty().MaximumLength(OfficialDocument.ContentSummaryMaxLength);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.WorkflowSteps).NotEmpty().WithMessage("OfficialDocument.WorkflowStepsRequired");
    }
}

public sealed class CreateOfficialDocumentHandler(
    IOfficialDocumentRepository documentRepository,
    IDocumentManagementUnitOfWork unitOfWork,
    IPermissionGuard permissionGuard,
    ICurrentUser currentUser,
    IAuditService auditService) : IRequestHandler<CreateOfficialDocumentCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(CreateOfficialDocumentCommand request, CancellationToken cancellationToken)
    {
        var authError = await permissionGuard.CheckAsync(
            Permissions.DocumentManagement.DocumentCreate,
            [ResourceScope.University],
            cancellationToken);

        if (authError is not null)
        {
            return AuthorizationFailure.To<Guid>(authError);
        }

        if (currentUser.UserId is not { } creatorUserId || creatorUserId == Guid.Empty)
        {
            return ServiceResult<Guid>.Unauthorized(new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var document = OfficialDocument.Create(
            request.Subject,
            request.ContentSummary,
            request.Type,
            creatorUserId,
            request.DepartmentCode,
            request.AttachmentUrl,
            DateTime.UtcNow);

        foreach (var step in request.WorkflowSteps)
        {
            document.AddWorkflowStep(step.StepType, step.AssigneeUserId);
        }

        if (request.SubmitImmediately)
        {
            document.SubmitToWorkflow();
        }

        await documentRepository.AddAsync(document, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "DocumentManagement.DocumentCreated",
            "DocumentManagement",
            nameof(OfficialDocument),
            document.Id.ToString(),
            newValues: new { document.Id, document.Subject, document.VerificationCode, document.Type },
            cancellationToken: cancellationToken);

        return ServiceResult<Guid>.Created(document.Id);
    }
}
