using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Courses;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.Courses;

public sealed record SetCourseStatusCommand(Guid Id, bool IsActive) : IRequest<ServiceResult<Unit>>;

public sealed class SetCourseStatusValidator : AbstractValidator<SetCourseStatusCommand>
{
    public SetCourseStatusValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Course.IdRequired");
    }
}

public sealed class SetCourseStatusHandler(
    ICourseRepository courseRepository,
    IDepartmentRepository departmentRepository,
    IAcademicUnitOfWork unitOfWork,
    IAuditService auditService,
    AcademicAccess access) : IRequestHandler<SetCourseStatusCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(SetCourseStatusCommand request, CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetByIdAsync(request.Id, cancellationToken);
        var department = course is null
            ? null
            : await departmentRepository.GetByIdAsync(course.DepartmentId, cancellationToken);

        var authError = await access.CheckCourseAsync(
            Permissions.Course.Update,
            department?.Id,
            department?.FacultyId,
            course is not null,
            cancellationToken);

        if (authError is not null)
        {
            return AuthorizationFailure.To<Unit>(authError);
        }

        if (course is null)
        {
            return ServiceResult<Unit>.NotFound(AcademicErrors.Course.NotFound());
        }

        if (request.IsActive)
        {
            course.Activate(DateTime.UtcNow);
        }
        else
        {
            course.Deactivate(DateTime.UtcNow);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "Course.StatusChanged",
            "Academic",
            nameof(Course),
            course.Id.ToString(),
            newValues: new { IsActive = request.IsActive },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
