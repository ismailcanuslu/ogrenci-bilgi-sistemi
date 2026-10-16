using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Courses;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.Courses;

public sealed record UpdateCourseCommand(
    Guid Id,
    string Name,
    string? Description,
    int DefaultEcts,
    int DefaultCredit,
    string? Prerequisites) : IRequest<ServiceResult<Unit>>;

public sealed class UpdateCourseValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Course.IdRequired");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Course.NameRequired")
            .MaximumLength(Course.NameMaxLength).WithMessage("Course.NameTooLong");

        RuleFor(x => x.DefaultEcts)
            .InclusiveBetween(1, 60).WithMessage("Course.InvalidEcts");

        RuleFor(x => x.DefaultCredit)
            .InclusiveBetween(0, 30).WithMessage("Course.InvalidCredit");

        RuleFor(x => x.Description)
            .MaximumLength(Course.DescriptionMaxLength).WithMessage("Course.DescriptionTooLong");

        RuleFor(x => x.Prerequisites)
            .MaximumLength(Course.PrerequisitesMaxLength).WithMessage("Course.PrerequisitesTooLong");
    }
}

public sealed class UpdateCourseHandler(
    ICourseRepository courseRepository,
    IDepartmentRepository departmentRepository,
    IAcademicUnitOfWork unitOfWork,
    IAuditService auditService,
    AcademicAccess access) : IRequestHandler<UpdateCourseCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
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

        course.Update(
            request.Name,
            request.Description,
            request.DefaultEcts,
            request.DefaultCredit,
            request.Prerequisites,
            DateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "Course.Updated",
            "Academic",
            nameof(Course),
            course.Id.ToString(),
            newValues: new { course.Name, course.DefaultEcts, course.DefaultCredit },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
