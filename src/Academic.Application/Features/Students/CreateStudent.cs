using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Curriculums;
using Academic.Domain.Students;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.Students;

public sealed record CreateStudentCommand(
    string StudentNumber,
    string FirstName,
    string LastName,
    string Email,
    Guid DepartmentId,
    Guid ProgramId,
    Guid CurriculumVersionId,
    int EnrollmentYear,
    DateTime EnrollmentDateUtc,
    Guid? UserId) : IRequest<ServiceResult<Guid>>;

public sealed class CreateStudentValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentValidator()
    {
        RuleFor(x => x.StudentNumber)
            .NotEmpty().WithMessage("Student.StudentNumberRequired")
            .MaximumLength(Student.StudentNumberMaxLength).WithMessage("Student.StudentNumberTooLong");

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

        RuleFor(x => x.DepartmentId).NotEmpty().WithMessage("Student.DepartmentRequired");
        RuleFor(x => x.ProgramId).NotEmpty().WithMessage("Student.ProgramRequired");
        RuleFor(x => x.CurriculumVersionId).NotEmpty().WithMessage("Student.CurriculumVersionRequired");
        RuleFor(x => x.EnrollmentYear).InclusiveBetween(1950, 2100).WithMessage("Student.InvalidEnrollmentYear");
    }
}

public sealed class CreateStudentHandler(
    IStudentRepository studentRepository,
    IDepartmentRepository departmentRepository,
    IProgramRepository programRepository,
    ICurriculumRepository curriculumRepository,
    IAcademicUnitOfWork unitOfWork,
    IAuditService auditService,
    AcademicAccess access) : IRequestHandler<CreateStudentCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var department = await departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
        if (department is null || !department.IsActive)
        {
            return ServiceResult<Guid>.NotFound(AcademicErrors.Department.NotFound());
        }

        var authError = await access.CheckStudentCreationAsync(
            Permissions.Student.Create, department.Id, department.FacultyId, cancellationToken);
        if (authError is not null)
        {
            return AuthorizationFailure.To<Guid>(authError);
        }

        var program = await programRepository.GetByIdAsync(request.ProgramId, cancellationToken);
        if (program is null || !program.IsActive || program.DepartmentId != department.Id)
        {
            return ServiceResult<Guid>.BadRequest(AcademicErrors.Program.NotFound());
        }

        var curriculum = await curriculumRepository.GetByVersionIdAsync(request.CurriculumVersionId, cancellationToken);
        if (curriculum is null || !curriculum.IsActive || curriculum.ProgramId != program.Id)
        {
            return ServiceResult<Guid>.BadRequest(AcademicErrors.Curriculum.NotFound());
        }

        var version = curriculum.Versions.FirstOrDefault(v => v.Id == request.CurriculumVersionId);
        if (version is null || version.Status != CurriculumVersionStatus.Published)
        {
            return ServiceResult<Guid>.BadRequest(AcademicErrors.Student.PublishedCurriculumRequired);
        }

        var normalizedNumber = Student.NormalizeStudentNumber(request.StudentNumber);
        if (await studentRepository.StudentNumberExistsAsync(normalizedNumber, null, cancellationToken))
        {
            return ServiceResult<Guid>.Conflict(AcademicErrors.Student.StudentNumberConflict);
        }

        var normalizedEmail = Student.NormalizeEmail(request.Email);
        if (await studentRepository.EmailExistsAsync(normalizedEmail, null, cancellationToken))
        {
            return ServiceResult<Guid>.Conflict(AcademicErrors.Student.EmailConflict);
        }

        var student = Student.Create(
            normalizedNumber,
            request.FirstName,
            request.LastName,
            normalizedEmail,
            department.Id,
            program.Id,
            version.Id,
            request.EnrollmentYear,
            request.EnrollmentDateUtc,
            request.UserId,
            DateTime.UtcNow);

        await studentRepository.AddAsync(student, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "Student.Created",
            "Academic",
            nameof(Student),
            student.Id.ToString(),
            newValues: new { student.StudentNumber, student.FirstName, student.LastName, student.DepartmentId, student.ProgramId },
            cancellationToken: cancellationToken);

        return ServiceResult<Guid>.Created(student.Id);
    }
}
