using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using ContinuingEducation.Application.Abstractions;
using ContinuingEducation.Domain.Programs;
using FluentValidation;
using MediatR;

namespace ContinuingEducation.Application.Features.Programs;

public sealed record EvaluateProgramApplicationCommand(
    Guid ApplicationId,
    decimal FinalGrade) : IRequest<ServiceResult<EvaluateProgramApplicationResponse>>;

public sealed record EvaluateProgramApplicationResponse(
    Guid ApplicationId,
    string Status,
    decimal FinalGrade,
    int AttendedHours,
    bool IsCertificateEarned,
    string? CertificateNumber);

public sealed class EvaluateProgramApplicationValidator : AbstractValidator<EvaluateProgramApplicationCommand>
{
    public EvaluateProgramApplicationValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.FinalGrade).InclusiveBetween(0, 100);
    }
}

public sealed class EvaluateProgramApplicationHandler(
    IProgramApplicationRepository applicationRepository,
    IEducationProgramRepository programRepository,
    IContinuingEducationUnitOfWork unitOfWork,
    IPermissionGuard permissionGuard,
    IAuditService auditService) : IRequestHandler<EvaluateProgramApplicationCommand, ServiceResult<EvaluateProgramApplicationResponse>>
{
    public async Task<ServiceResult<EvaluateProgramApplicationResponse>> Handle(
        EvaluateProgramApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var authError = await permissionGuard.CheckAsync(
            Permissions.ContinuingEducation.GradeManage,
            [ResourceScope.University],
            cancellationToken);

        if (authError is not null)
        {
            return AuthorizationFailure.To<EvaluateProgramApplicationResponse>(authError);
        }

        var application = await applicationRepository.GetByIdAsync(request.ApplicationId, cancellationToken);
        if (application is null)
        {
            return ServiceResult<EvaluateProgramApplicationResponse>.NotFound(
                new Error("ProgramApplication.NotFound", ErrorType.NotFound));
        }

        var program = await programRepository.GetByIdAsync(application.ProgramId, cancellationToken);
        if (program is null)
        {
            return ServiceResult<EvaluateProgramApplicationResponse>.NotFound(
                new Error("EducationProgram.NotFound", ErrorType.NotFound));
        }

        application.FinalizeEvaluation(
            request.FinalGrade,
            program.TotalHours,
            program.MinAttendancePercentage,
            program.MinPassingGrade,
            DateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "ContinuingEducation.ApplicationEvaluated",
            "ContinuingEducation",
            nameof(ProgramApplication),
            application.Id.ToString(),
            newValues: new
            {
                application.Id,
                Status = application.Status.ToString(),
                FinalGrade = application.FinalGrade,
                application.AttendedHours,
                application.IsCertificateEarned,
                application.CertificateNumber
            },
            cancellationToken: cancellationToken);

        var response = new EvaluateProgramApplicationResponse(
            application.Id,
            application.Status.ToString(),
            application.FinalGrade!.Value,
            application.AttendedHours,
            application.IsCertificateEarned,
            application.CertificateNumber);

        return ServiceResult<EvaluateProgramApplicationResponse>.Ok(response);
    }
}
