using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Courses;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.CourseOfferings;

public sealed record SetOfferingRegistrationStatusCommand(
    Guid OfferingId,
    bool IsOpen) : IRequest<ServiceResult<Unit>>;

public sealed class SetOfferingRegistrationStatusValidator : AbstractValidator<SetOfferingRegistrationStatusCommand>
{
    public SetOfferingRegistrationStatusValidator()
    {
        RuleFor(x => x.OfferingId).NotEmpty().WithMessage("CourseOffering.IdRequired");
    }
}

public sealed class SetOfferingRegistrationStatusHandler(
    ICourseOfferingRepository offeringRepository,
    ICourseRepository courseRepository,
    IDepartmentRepository departmentRepository,
    IAcademicUnitOfWork unitOfWork,
    IAuditService auditService,
    AcademicAccess access) : IRequestHandler<SetOfferingRegistrationStatusCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(SetOfferingRegistrationStatusCommand request, CancellationToken cancellationToken)
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

        offering.SetRegistrationStatus(request.IsOpen, DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "CourseOffering.RegistrationStatusChanged",
            "Academic",
            nameof(CourseOffering),
            offering.Id.ToString(),
            newValues: new { IsRegistrationOpen = request.IsOpen },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
