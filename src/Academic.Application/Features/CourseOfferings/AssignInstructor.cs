using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Courses;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.CourseOfferings;

public sealed record AssignInstructorCommand(
    Guid OfferingId,
    Guid InstructorId,
    bool IsPrimary = true,
    string? RoleOrTitle = null) : IRequest<ServiceResult<Unit>>;

public sealed class AssignInstructorValidator : AbstractValidator<AssignInstructorCommand>
{
    public AssignInstructorValidator()
    {
        RuleFor(x => x.OfferingId).NotEmpty().WithMessage("CourseOffering.IdRequired");
        RuleFor(x => x.InstructorId).NotEmpty().WithMessage("CourseOffering.InstructorRequired");
    }
}

public sealed class AssignInstructorHandler(
    ICourseOfferingRepository offeringRepository,
    ICourseRepository courseRepository,
    IDepartmentRepository departmentRepository,
    IAcademicUnitOfWork unitOfWork,
    IAuditService auditService,
    AcademicAccess access) : IRequestHandler<AssignInstructorCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(AssignInstructorCommand request, CancellationToken cancellationToken)
    {
        var offering = await offeringRepository.GetByIdAsync(request.OfferingId, true, cancellationToken);
        var course = offering is null
            ? null
            : await courseRepository.GetByIdAsync(offering.CourseId, cancellationToken);
        var department = course is null
            ? null
            : await departmentRepository.GetByIdAsync(course.DepartmentId, cancellationToken);

        var authError = await access.CheckCourseOfferingAsync(
            Permissions.Course.AssignmentManage,
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

        // Eğitmenin atanacağı oturum saatleri (kendisine atanmış veya birincil eğitmen ise genel oturumlar) için çakışma kontrolü
        var relevantSchedules = offering.Schedules.Where(s => s.InstructorId == request.InstructorId || (request.IsPrimary && s.InstructorId == null));
        foreach (var schedule in relevantSchedules)
        {
            var conflict = await offeringRepository.HasInstructorConflictAsync(
                request.InstructorId,
                offering.AcademicTermId,
                schedule.DayOfWeek,
                schedule.StartTime,
                schedule.EndTime,
                offering.Id,
                cancellationToken);

            if (conflict)
            {
                return ServiceResult<Unit>.Conflict(AcademicErrors.CourseOffering.InstructorScheduleConflict);
            }
        }

        var previousPrimaryInstructor = offering.PrimaryInstructorId;
        offering.AssignInstructor(request.InstructorId, request.IsPrimary, request.RoleOrTitle, DateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "CourseOffering.InstructorAssigned",
            "Academic",
            nameof(CourseOffering),
            offering.Id.ToString(),
            newValues: new { PreviousPrimaryInstructor = previousPrimaryInstructor, request.InstructorId, request.IsPrimary, request.RoleOrTitle },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
