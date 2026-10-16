using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Students;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.Students;

public sealed record SetStudentPrimaryProgramCommand(
    Guid StudentId,
    Guid ProgramEnrollmentId) : IRequest<ServiceResult<Unit>>;

public sealed class SetStudentPrimaryProgramValidator : AbstractValidator<SetStudentPrimaryProgramCommand>
{
    public SetStudentPrimaryProgramValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty().WithMessage("Student.IdRequired");
        RuleFor(x => x.ProgramEnrollmentId).NotEmpty().WithMessage("Student.ProgramEnrollmentRequired");
    }
}

public sealed class SetStudentPrimaryProgramHandler(
    IStudentRepository studentRepository,
    IDepartmentRepository departmentRepository,
    IAcademicUnitOfWork unitOfWork,
    IAuditService auditService,
    AcademicAccess access) : IRequestHandler<SetStudentPrimaryProgramCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(SetStudentPrimaryProgramCommand request, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
        var department = student is null
            ? null
            : await departmentRepository.GetByIdAsync(student.DepartmentId, cancellationToken);

        // Öğrencinin kendisi de aktif programını seçebilir veya yetkili personel
        var authError = await access.CheckStudentAsync(
            Permissions.Student.Read,
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

        student.SetPrimaryProgram(request.ProgramEnrollmentId, DateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "Student.PrimaryProgramChanged",
            "Academic",
            nameof(Student),
            student.Id.ToString(),
            newValues: new { request.ProgramEnrollmentId, student.ProgramId, student.DepartmentId },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
