using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Courses;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.CourseOfferings;

public sealed record AddOfferingScheduleCommand(
    Guid OfferingId,
    SessionType SessionType,
    string BuildingOrFaculty,
    string Classroom,
    DayOfWeek DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    Guid? InstructorId = null,
    bool IsOnline = false) : IRequest<ServiceResult<Guid>>;

public sealed class AddOfferingScheduleValidator : AbstractValidator<AddOfferingScheduleCommand>
{
    public AddOfferingScheduleValidator()
    {
        RuleFor(x => x.OfferingId).NotEmpty().WithMessage("CourseOffering.IdRequired");
        RuleFor(x => x.SessionType).IsInEnum().WithMessage("CourseOfferingSchedule.InvalidSessionType");
        RuleFor(x => x.BuildingOrFaculty)
            .NotEmpty().WithMessage("CourseOfferingSchedule.BuildingOrFacultyRequired")
            .MaximumLength(CourseOfferingSchedule.BuildingOrFacultyMaxLength).WithMessage("CourseOfferingSchedule.BuildingOrFacultyTooLong");
        RuleFor(x => x.Classroom)
            .NotEmpty().WithMessage("CourseOfferingSchedule.ClassroomRequired")
            .MaximumLength(CourseOfferingSchedule.ClassroomMaxLength).WithMessage("CourseOfferingSchedule.ClassroomTooLong");
        RuleFor(x => x.DayOfWeek).IsInEnum().WithMessage("CourseOfferingSchedule.InvalidDayOfWeek");
        RuleFor(x => x.StartTime).LessThan(x => x.EndTime).WithMessage("CourseOfferingSchedule.InvalidTimeRange");
    }
}

public sealed class AddOfferingScheduleHandler(
    ICourseOfferingRepository offeringRepository,
    ICourseRepository courseRepository,
    IDepartmentRepository departmentRepository,
    IAcademicUnitOfWork unitOfWork,
    IAuditService auditService,
    AcademicAccess access) : IRequestHandler<AddOfferingScheduleCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(AddOfferingScheduleCommand request, CancellationToken cancellationToken)
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
            return AuthorizationFailure.To<Guid>(authError);
        }

        if (offering is null)
        {
            return ServiceResult<Guid>.NotFound(AcademicErrors.CourseOffering.NotFound());
        }

        if (!request.IsOnline)
        {
            var classroomConflict = await offeringRepository.HasClassroomConflictAsync(
                request.BuildingOrFaculty,
                request.Classroom,
                offering.AcademicTermId,
                request.DayOfWeek,
                request.StartTime,
                request.EndTime,
                offering.Id,
                cancellationToken);

            if (classroomConflict)
            {
                return ServiceResult<Guid>.Conflict(AcademicErrors.CourseOffering.ClassroomConflict);
            }
        }

        var targetInstructorId = request.InstructorId ?? offering.PrimaryInstructorId;
        if (targetInstructorId.HasValue)
        {
            var instructorConflict = await offeringRepository.HasInstructorConflictAsync(
                targetInstructorId.Value,
                offering.AcademicTermId,
                request.DayOfWeek,
                request.StartTime,
                request.EndTime,
                offering.Id,
                cancellationToken);

            if (instructorConflict)
            {
                return ServiceResult<Guid>.Conflict(AcademicErrors.CourseOffering.InstructorScheduleConflict);
            }
        }

        var schedule = offering.AddSchedule(
            request.SessionType,
            request.BuildingOrFaculty,
            request.Classroom,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.InstructorId,
            request.IsOnline);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "CourseOffering.ScheduleAdded",
            "Academic",
            nameof(CourseOffering),
            offering.Id.ToString(),
            newValues: new
            {
                schedule.Id,
                request.SessionType,
                request.BuildingOrFaculty,
                request.Classroom,
                request.DayOfWeek,
                request.StartTime,
                request.EndTime,
                request.InstructorId,
                request.IsOnline
            },
            cancellationToken: cancellationToken);

        return ServiceResult<Guid>.Created(schedule.Id);
    }
}
