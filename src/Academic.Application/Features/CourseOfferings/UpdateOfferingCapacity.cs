using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Courses;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.CourseOfferings;

public sealed record UpdateOfferingCapacityCommand(
    Guid OfferingId,
    int Capacity) : IRequest<ServiceResult<Unit>>;

public sealed class UpdateOfferingCapacityValidator : AbstractValidator<UpdateOfferingCapacityCommand>
{
    public UpdateOfferingCapacityValidator()
    {
        RuleFor(x => x.OfferingId).NotEmpty().WithMessage("CourseOffering.IdRequired");
        RuleFor(x => x.Capacity).GreaterThanOrEqualTo(1).WithMessage("CourseOffering.InvalidCapacity");
    }
}

public sealed class UpdateOfferingCapacityHandler(
    ICourseOfferingRepository offeringRepository,
    ICourseRepository courseRepository,
    IDepartmentRepository departmentRepository,
    IAcademicUnitOfWork unitOfWork,
    IAuditService auditService,
    AcademicAccess access) : IRequestHandler<UpdateOfferingCapacityCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(UpdateOfferingCapacityCommand request, CancellationToken cancellationToken)
    {
        var offering = await offeringRepository.GetByIdAsync(request.OfferingId, false, cancellationToken);
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

        var previousCapacity = offering.Capacity;
        offering.UpdateCapacity(request.Capacity, DateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "CourseOffering.CapacityUpdated",
            "Academic",
            nameof(CourseOffering),
            offering.Id.ToString(),
            newValues: new { PreviousCapacity = previousCapacity, NewCapacity = request.Capacity },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
