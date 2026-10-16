using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using ContinuingEducation.Application.Abstractions;
using ContinuingEducation.Domain.Programs;
using FluentValidation;
using MediatR;

namespace ContinuingEducation.Application.Features.Programs;

public sealed record RecordProgramAttendanceCommand(
    Guid ApplicationId,
    int AttendedHours) : IRequest<ServiceResult<Unit>>;

public sealed class RecordProgramAttendanceValidator : AbstractValidator<RecordProgramAttendanceCommand>
{
    public RecordProgramAttendanceValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.AttendedHours).GreaterThanOrEqualTo(0);
    }
}

public sealed class RecordProgramAttendanceHandler(
    IProgramApplicationRepository applicationRepository,
    IContinuingEducationUnitOfWork unitOfWork,
    IPermissionGuard permissionGuard,
    IAuditService auditService) : IRequestHandler<RecordProgramAttendanceCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(RecordProgramAttendanceCommand request, CancellationToken cancellationToken)
    {
        var authError = await permissionGuard.CheckAsync(
            Permissions.ContinuingEducation.AttendanceManage,
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

        application.AddAttendedHours(request.AttendedHours);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "ContinuingEducation.AttendanceRecorded",
            "ContinuingEducation",
            nameof(ProgramApplication),
            application.Id.ToString(),
            newValues: new { application.Id, application.AttendedHours, Added = request.AttendedHours },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
