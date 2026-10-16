using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using DocumentManagement.Application.Abstractions;
using DocumentManagement.Domain.Documents;
using FluentValidation;
using MediatR;

namespace DocumentManagement.Application.Features.Documents;

public sealed record ProcessDocumentStepCommand(
    Guid DocumentId,
    Guid StepId,
    bool Approve,
    string? Note = null) : IRequest<ServiceResult<Unit>>;

public sealed class ProcessDocumentStepValidator : AbstractValidator<ProcessDocumentStepCommand>
{
    public ProcessDocumentStepValidator()
    {
        RuleFor(x => x.DocumentId).NotEmpty();
        RuleFor(x => x.StepId).NotEmpty();
        RuleFor(x => x.Note).MaximumLength(DocumentWorkflowStep.NoteMaxLength);
    }
}

public sealed class ProcessDocumentStepHandler(
    IOfficialDocumentRepository documentRepository,
    IDocumentManagementUnitOfWork unitOfWork,
    IPermissionGuard permissionGuard,
    ICurrentUser currentUser,
    IAuditService auditService) : IRequestHandler<ProcessDocumentStepCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(ProcessDocumentStepCommand request, CancellationToken cancellationToken)
    {
        var authError = await permissionGuard.CheckAsync(
            Permissions.DocumentManagement.DocumentSign,
            [ResourceScope.University],
            cancellationToken);

        if (authError is not null)
        {
            return AuthorizationFailure.To<Unit>(authError);
        }

        if (currentUser.UserId is not { } currentUserId || currentUserId == Guid.Empty)
        {
            return ServiceResult<Unit>.Unauthorized(new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var document = await documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
        if (document is null)
        {
            return ServiceResult<Unit>.NotFound(new Error("OfficialDocument.NotFound", ErrorType.NotFound));
        }

        document.ProcessStep(request.StepId, currentUserId, request.Approve, request.Note, DateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "DocumentManagement.StepProcessed",
            "DocumentManagement",
            nameof(OfficialDocument),
            document.Id.ToString(),
            newValues: new { document.Id, request.StepId, request.Approve, Status = document.Status.ToString(), document.DocumentNumber },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
