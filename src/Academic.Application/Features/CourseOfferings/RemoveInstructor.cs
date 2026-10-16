using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Courses;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.CourseOfferings;

public sealed record RemoveInstructorCommand(
    Guid OfferingId,
    Guid InstructorId) : IRequest<ServiceResult<Unit>>;

public sealed class RemoveInstructorValidator : AbstractValidator<RemoveInstructorCommand>
{
    public RemoveInstructorValidator()
    {
        RuleFor(x => x.OfferingId).NotEmpty().WithMessage("CourseOffering.IdRequired");
        RuleFor(x => x.InstructorId).NotEmpty().WithMessage("CourseOffering.InstructorRequired");
    }
}

public sealed class RemoveInstructorHandler(
    ICourseOfferingRepository offeringRepository,
    ICourseRepository courseRepository,
    IDepartmentRepository departmentRepository,
    IAcademicUnitOfWork unitOfWork,
    IAuditService auditService,
    AcademicAccess access) : IRequestHandler<RemoveInstructorCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(RemoveInstructorCommand request, CancellationToken cancellationToken)
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

        offering.RemoveInstructor(request.InstructorId, DateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "CourseOffering.InstructorRemoved",
            "Academic",
            nameof(CourseOffering),
            offering.Id.ToString(),
            newValues: new { request.InstructorId },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
