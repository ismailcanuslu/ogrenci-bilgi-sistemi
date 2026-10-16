using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using ContinuingEducation.Application.Abstractions;
using ContinuingEducation.Domain.Programs;
using FluentValidation;
using MediatR;

namespace ContinuingEducation.Application.Features.Programs;

public sealed record ApplyToEducationProgramCommand(
    Guid ProgramId) : IRequest<ServiceResult<Guid>>;

public sealed class ApplyToEducationProgramValidator : AbstractValidator<ApplyToEducationProgramCommand>
{
    public ApplyToEducationProgramValidator()
    {
        RuleFor(x => x.ProgramId).NotEmpty();
    }
}

public sealed class ApplyToEducationProgramHandler(
    IEducationProgramRepository programRepository,
    IProgramApplicationRepository applicationRepository,
    IContinuingEducationUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IAuditService auditService) : IRequestHandler<ApplyToEducationProgramCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(ApplyToEducationProgramCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } applicantUserId || applicantUserId == Guid.Empty)
        {
            return ServiceResult<Guid>.Unauthorized(new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var program = await programRepository.GetByIdAsync(request.ProgramId, cancellationToken);
        if (program is null)
        {
            return ServiceResult<Guid>.NotFound(new Error("EducationProgram.NotFound", ErrorType.NotFound));
        }

        if (program.Status != CourseProgramStatus.Active)
        {
            return ServiceResult<Guid>.Failure(new Error("EducationProgram.NotActive", ErrorType.Validation));
        }

        var existing = await applicationRepository.GetByProgramAndApplicantAsync(
            request.ProgramId,
            applicantUserId,
            cancellationToken);

        if (existing is not null)
        {
            return ServiceResult<Guid>.Conflict(new Error("ProgramApplication.AlreadyExists", ErrorType.Conflict));
        }

        var application = ProgramApplication.Create(
            request.ProgramId,
            applicantUserId,
            DateTime.UtcNow);

        await applicationRepository.AddAsync(application, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "ContinuingEducation.ApplicationCreated",
            "ContinuingEducation",
            nameof(ProgramApplication),
            application.Id.ToString(),
            newValues: new { application.Id, application.ProgramId, application.ApplicantUserId },
            cancellationToken: cancellationToken);

        return ServiceResult<Guid>.Created(application.Id);
    }
}
