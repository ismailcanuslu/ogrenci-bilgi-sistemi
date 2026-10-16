using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Students;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.Students;

public sealed record UpdateStudentCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email) : IRequest<ServiceResult<Unit>>;

public sealed class UpdateStudentValidator : AbstractValidator<UpdateStudentCommand>
{
    public UpdateStudentValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Student.IdRequired");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Student.FirstNameRequired")
            .MaximumLength(Student.NameMaxLength).WithMessage("Student.FirstNameTooLong");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Student.LastNameRequired")
            .MaximumLength(Student.NameMaxLength).WithMessage("Student.LastNameTooLong");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Student.EmailRequired")
            .EmailAddress().WithMessage("Student.InvalidEmail")
            .MaximumLength(Student.EmailMaxLength).WithMessage("Student.EmailTooLong");
    }
}

public sealed class UpdateStudentHandler(
    IStudentRepository studentRepository,
    IDepartmentRepository departmentRepository,
    IAcademicUnitOfWork unitOfWork,
    IAuditService auditService,
    AcademicAccess access) : IRequestHandler<UpdateStudentCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(request.Id, cancellationToken);
        var department = student is null
            ? null
            : await departmentRepository.GetByIdAsync(student.DepartmentId, cancellationToken);

        var authError = await access.CheckStudentAsync(
            Permissions.Student.Update,
            student?.UserId,
            department?.Id,
            department?.FacultyId,
            student is not null,
            cancellationToken);

        if (authError is not null)
        {
            return AuthorizationFailure.To<Unit>(authError);
        }

        if (student is null)
        {
            return ServiceResult<Unit>.NotFound(AcademicErrors.Student.NotFound());
        }

        var normalizedEmail = Student.NormalizeEmail(request.Email);
        if (await studentRepository.EmailExistsAsync(normalizedEmail, student.Id, cancellationToken))
        {
            return ServiceResult<Unit>.Conflict(AcademicErrors.Student.EmailConflict);
        }

        student.UpdateProfile(request.FirstName, request.LastName, normalizedEmail, DateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "Student.Updated",
            "Academic",
            nameof(Student),
            student.Id.ToString(),
            newValues: new { student.FirstName, student.LastName, student.Email },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
