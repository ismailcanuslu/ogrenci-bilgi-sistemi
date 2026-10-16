using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Courses;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.CourseOfferings;

public sealed record RemoveOfferingScheduleCommand(
    Guid OfferingId,
    Guid ScheduleId) : IRequest<ServiceResult<Unit>>;

public sealed class RemoveOfferingScheduleValidator : AbstractValidator<RemoveOfferingScheduleCommand>
{
    public RemoveOfferingScheduleValidator()
    {
        RuleFor(x => x.OfferingId).NotEmpty().WithMessage("CourseOffering.IdRequired");
        RuleFor(x => x.ScheduleId).NotEmpty().WithMessage("CourseOfferingSchedule.IdRequired");
    }
}

public sealed class RemoveOfferingScheduleHandler(
    ICourseOfferingRepository offeringRepository,
    ICourseRepository courseRepository,
    IDepartmentRepository departmentRepository,
    IAcademicUnitOfWork unitOfWork,
    IAuditService auditService,
    AcademicAccess access) : IRequestHandler<RemoveOfferingScheduleCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(RemoveOfferingScheduleCommand request, CancellationToken cancellationToken)
    {
        var offering = await offeringRepository.GetByIdAsync(request.OfferingId, true, cancellationToken);
        var course = offering is null
            ? null
            : await courseRepository.GetByIdAsync(offering.CourseId, cancellationToken);
        var department = course is null
            ? null
            : await departmentRepository.GetByIdAsync(course.DepartmentId, cancellationToken);

        var authError = await access.CheckCourseOfferingAsync(
            Permissions.Course.ScheduleManage,
            offering?.Id ?? Guid.Empty,
            offering?.PrimaryInstructorId,
            department?.Id,
            department?.FacultyId,
            offering is not null,
            cancellationToken);

        if (authError is not null)
        {
            return AuthorizationFailure.To<Unit>(authError);
        }

        if (offering is null)
        {
            return ServiceResult<Unit>.NotFound(AcademicErrors.CourseOffering.NotFound());
        }

        offering.RemoveSchedule(request.ScheduleId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "CourseOffering.ScheduleRemoved",
            "Academic",
            nameof(CourseOffering),
            offering.Id.ToString(),
            newValues: new { ScheduleId = request.ScheduleId },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
