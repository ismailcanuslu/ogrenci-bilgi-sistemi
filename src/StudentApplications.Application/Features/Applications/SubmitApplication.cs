using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;
using StudentApplications.Application.Abstractions;
using StudentApplications.Domain.Applications;

namespace StudentApplications.Application.Features.Applications;

public sealed record SubmitApplicationCommand(
    ApplicationType Type,
    Guid? TargetDepartmentId,
    string? TargetInstitution,
    string Details,
    string? AttachmentUrl,
    decimal? Gpa) : IRequest<ServiceResult<Guid>>;

public sealed class SubmitApplicationValidator : AbstractValidator<SubmitApplicationCommand>
{
    public SubmitApplicationValidator()
    {
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Details).NotEmpty().MaximumLength(StudentApplication.DetailsMaxLength);
        RuleFor(x => x.TargetInstitution).MaximumLength(StudentApplication.TargetInstitutionMaxLength);
        When(x => x.Type == ApplicationType.DoubleMajor || x.Type == ApplicationType.Minor, () =>
        {
            RuleFor(x => x.TargetDepartmentId).NotEmpty().WithMessage("StudentApplication.TargetDepartmentRequired");
        });
    }
}

public sealed class SubmitApplicationHandler(
    IStudentApplicationRepository applicationRepository,
    IStudentApplicationsUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IAuditService auditService) : IRequestHandler<SubmitApplicationCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(SubmitApplicationCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } studentId || studentId == Guid.Empty)
        {
            return ServiceResult<Guid>.Unauthorized(new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var application = StudentApplication.Create(
            studentId,
            request.Type,
            request.TargetDepartmentId,
            request.TargetInstitution,
            request.Details,
            request.AttachmentUrl,
            request.Gpa,
            DateTime.UtcNow);

        await applicationRepository.AddAsync(application, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "StudentApplications.ApplicationSubmitted",
            "StudentApplications",
            nameof(StudentApplication),
            application.Id.ToString(),
            newValues: new { application.Id, application.ApplicationNumber, application.Type, application.StudentId },
            cancellationToken: cancellationToken);

        return ServiceResult<Guid>.Created(application.Id);
    }
}
