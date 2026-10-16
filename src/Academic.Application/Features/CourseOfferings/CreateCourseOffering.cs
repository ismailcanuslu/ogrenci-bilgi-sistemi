using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Courses;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.CourseOfferings;

public sealed record CreateCourseOfferingCommand(
    Guid CourseId,
    Guid AcademicTermId,
    int Section,
    int Capacity,
    Guid? PrimaryInstructorId) : IRequest<ServiceResult<Guid>>;

public sealed class CreateCourseOfferingValidator : AbstractValidator<CreateCourseOfferingCommand>
{
    public CreateCourseOfferingValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty().WithMessage("CourseOffering.CourseRequired");
        RuleFor(x => x.AcademicTermId).NotEmpty().WithMessage("CourseOffering.AcademicTermRequired");
        RuleFor(x => x.Section).GreaterThanOrEqualTo(1).WithMessage("CourseOffering.InvalidSection");
        RuleFor(x => x.Capacity).GreaterThanOrEqualTo(1).WithMessage("CourseOffering.InvalidCapacity");
    }
}

public sealed class CreateCourseOfferingHandler(
    ICourseOfferingRepository offeringRepository,
    ICourseRepository courseRepository,
    IDepartmentRepository departmentRepository,
    IAcademicYearRepository academicYearRepository,
    IAcademicUnitOfWork unitOfWork,
    IAuditService auditService,
    AcademicAccess access) : IRequestHandler<CreateCourseOfferingCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(CreateCourseOfferingCommand request, CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null || !course.IsActive)
        {
            return ServiceResult<Guid>.NotFound(AcademicErrors.Course.NotFound());
        }

        var department = await departmentRepository.GetByIdAsync(course.DepartmentId, cancellationToken);
        if (department is null || !department.IsActive)
        {
            return ServiceResult<Guid>.NotFound(AcademicErrors.Department.NotFound());
        }

        var authError = await access.CheckCourseCreationAsync(
            Permissions.Course.Create, department.Id, department.FacultyId, cancellationToken);
        if (authError is not null)
        {
            return AuthorizationFailure.To<Guid>(authError);
        }

        var term = await academicYearRepository.GetTermByIdAsync(request.AcademicTermId, cancellationToken);
        if (term is null)
        {
            return ServiceResult<Guid>.NotFound(AcademicErrors.AcademicTerm.NotFound());
        }

        if (await offeringRepository.SectionExistsAsync(course.Id, term.Id, request.Section, null, cancellationToken))
        {
            return ServiceResult<Guid>.Conflict(AcademicErrors.CourseOffering.SectionConflict);
        }

        var offering = CourseOffering.Create(
            course.Id,
            term.Id,
            request.Section,
            request.Capacity,
            request.PrimaryInstructorId,
            DateTime.UtcNow);

        await offeringRepository.AddAsync(offering, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "CourseOffering.Created",
            "Academic",
            nameof(CourseOffering),
            offering.Id.ToString(),
            newValues: new { offering.CourseId, offering.AcademicTermId, offering.Section, offering.Capacity, offering.PrimaryInstructorId },
            cancellationToken: cancellationToken);

        return ServiceResult<Guid>.Created(offering.Id);
    }
}
