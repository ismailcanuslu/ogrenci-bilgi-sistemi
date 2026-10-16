using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Courses;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.Courses;

public sealed record CreateCourseCommand(
    Guid DepartmentId,
    string Code,
    string Name,
    string? Description,
    int DefaultEcts,
    int DefaultCredit,
    string? Prerequisites) : IRequest<ServiceResult<Guid>>;

public sealed class CreateCourseValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseValidator()
    {
        RuleFor(x => x.DepartmentId).NotEmpty().WithMessage("Course.DepartmentRequired");
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Course.CodeRequired")
            .MaximumLength(Course.CodeMaxLength).WithMessage("Course.CodeTooLong");

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

public sealed class CreateCourseHandler(
    ICourseRepository courseRepository,
    IDepartmentRepository departmentRepository,
    IAcademicUnitOfWork unitOfWork,
    IAuditService auditService,
    AcademicAccess access) : IRequestHandler<CreateCourseCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var department = await departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
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

        var normalizedCode = Course.NormalizeCode(request.Code);
        if (await courseRepository.CodeExistsAsync(normalizedCode, null, cancellationToken))
        {
            return ServiceResult<Guid>.Conflict(AcademicErrors.Course.CodeConflict);
        }

        var course = Course.Create(
            department.Id,
            normalizedCode,
            request.Name,
            request.Description,
            request.DefaultEcts,
            request.DefaultCredit,
            request.Prerequisites,
            DateTime.UtcNow);

        await courseRepository.AddAsync(course, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "Course.Created",
            "Academic",
            nameof(Course),
            course.Id.ToString(),
            newValues: new { course.Code, course.Name, course.DefaultEcts, course.DefaultCredit },
            cancellationToken: cancellationToken);

        return ServiceResult<Guid>.Created(course.Id);
    }
}
