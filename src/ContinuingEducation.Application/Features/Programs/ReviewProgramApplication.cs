using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using ContinuingEducation.Application.Abstractions;
using ContinuingEducation.Domain.Programs;
using FluentValidation;
using MediatR;

namespace ContinuingEducation.Application.Features.Programs;

public sealed record ReviewProgramApplicationCommand(
    Guid ApplicationId,
    bool Approve,
    string? Reason = null) : IRequest<ServiceResult<Unit>>;

public sealed class ReviewProgramApplicationValidator : AbstractValidator<ReviewProgramApplicationCommand>
{
    public ReviewProgramApplicationValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
    }
}

public sealed class ReviewProgramApplicationHandler(
    IProgramApplicationRepository applicationRepository,
    IContinuingEducationUnitOfWork unitOfWork,
    IPermissionGuard permissionGuard,
    IAuditService auditService) : IRequestHandler<ReviewProgramApplicationCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(ReviewProgramApplicationCommand request, CancellationToken cancellationToken)
    {
        var authError = await permissionGuard.CheckAsync(
            Permissions.ContinuingEducation.ProgramManage,
            [ResourceScope.University],
            cancellationToken);

        if (authError is not null)
        {
            return AuthorizationFailure.To<Unit>(authError);
        }

        var application = await applicationRepository.GetByIdAsync(request.ApplicationId, cancellationToken);
        if (application is null)
        {
            return ServiceResult<Unit>.NotFound(new Error("ProgramApplication.NotFound", ErrorType.NotFound));
        }

        if (request.Approve)
        {
            application.Approve(DateTime.UtcNow);
        }
        else
        {
            application.Reject(request.Reason, DateTime.UtcNow);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "ContinuingEducation.ApplicationReviewed",
            "ContinuingEducation",
            nameof(ProgramApplication),
            application.Id.ToString(),
            newValues: new { application.Id, Status = application.Status.ToString(), request.Approve, request.Reason },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
