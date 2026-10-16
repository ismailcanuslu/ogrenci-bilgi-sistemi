using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;
using StudentApplications.Application.Abstractions;
using StudentApplications.Domain.Applications;

namespace StudentApplications.Application.Features.Applications;

public sealed record ReviewApplicationCommand(
    Guid ApplicationId,
    bool Approve,
    string? Notes = null) : IRequest<ServiceResult<Unit>>;

public sealed class ReviewApplicationValidator : AbstractValidator<ReviewApplicationCommand>
{
    public ReviewApplicationValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(StudentApplication.ReviewNotesMaxLength);
        When(x => !x.Approve, () =>
        {
            RuleFor(x => x.Notes).NotEmpty().WithMessage("StudentApplication.RejectionReasonRequired");
        });
    }
}

public sealed class ReviewApplicationHandler(
    IStudentApplicationRepository applicationRepository,
    IStudentApplicationsUnitOfWork unitOfWork,
    IPermissionGuard permissionGuard,
    ICurrentUser currentUser,
    IAuditService auditService) : IRequestHandler<ReviewApplicationCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(ReviewApplicationCommand request, CancellationToken cancellationToken)
    {
        var authError = await permissionGuard.CheckAsync(
            Permissions.Student.EnrollmentManage,
            [ResourceScope.University],
            cancellationToken);

        if (authError is not null)
        {
            return AuthorizationFailure.To<Unit>(authError);
        }

        if (currentUser.UserId is not { } reviewerUserId || reviewerUserId == Guid.Empty)
        {
            return ServiceResult<Unit>.Unauthorized(new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var application = await applicationRepository.GetByIdAsync(request.ApplicationId, cancellationToken);
        if (application is null)
        {
            return ServiceResult<Unit>.NotFound(new Error("StudentApplication.NotFound", ErrorType.NotFound));
        }

        if (request.Approve)
        {
            application.Approve(reviewerUserId, request.Notes, DateTime.UtcNow);
        }
        else
        {
            application.Reject(reviewerUserId, request.Notes!, DateTime.UtcNow);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "StudentApplications.ApplicationReviewed",
            "StudentApplications",
            nameof(StudentApplication),
            application.Id.ToString(),
            newValues: new { application.Id, application.ApplicationNumber, State = application.State.ToString(), request.Approve, request.Notes },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
