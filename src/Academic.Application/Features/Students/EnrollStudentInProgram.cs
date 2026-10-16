using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Students;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.Students;

public sealed record EnrollStudentInProgramCommand(
    Guid StudentId,
    Guid DepartmentId,
    Guid ProgramId,
    Guid CurriculumVersionId,
    EnrollmentType EnrollmentType,
    int EnrollmentYear,
    bool IsPrimary = false,
    string? Notes = null) : IRequest<ServiceResult<Guid>>;

public sealed class EnrollStudentInProgramValidator : AbstractValidator<EnrollStudentInProgramCommand>
{
    public EnrollStudentInProgramValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty().WithMessage("Student.IdRequired");
        RuleFor(x => x.DepartmentId).NotEmpty().WithMessage("Student.DepartmentRequired");
        RuleFor(x => x.ProgramId).NotEmpty().WithMessage("Student.ProgramRequired");
        RuleFor(x => x.CurriculumVersionId).NotEmpty().WithMessage("Student.CurriculumVersionRequired");
        RuleFor(x => x.EnrollmentType).IsInEnum().WithMessage("Student.InvalidEnrollmentType");
        RuleFor(x => x.EnrollmentYear).InclusiveBetween(1950, 2100).WithMessage("Student.InvalidEnrollmentYear");
    }
}

public sealed class EnrollStudentInProgramHandler(
    IStudentRepository studentRepository,
    IDepartmentRepository departmentRepository,
    IAcademicUnitOfWork unitOfWork,
    IAuditService auditService,
    AcademicAccess access) : IRequestHandler<EnrollStudentInProgramCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(EnrollStudentInProgramCommand request, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
        var targetDepartment = await departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);

        var authError = await access.CheckStudentAsync(
            Permissions.Student.Update,
            student?.UserId,
            targetDepartment?.Id,
            targetDepartment?.FacultyId,
            student is not null && targetDepartment is not null,
            cancellationToken);

        if (authError is not null)
        {
            return AuthorizationFailure.To<Guid>(authError);
        }

        if (student is null)
        {
            return ServiceResult<Guid>.NotFound(AcademicErrors.Student.NotFound());
        }

        var enrollment = student.EnrollInProgram(
            request.DepartmentId,
            request.ProgramId,
            request.CurriculumVersionId,
            request.EnrollmentType,
            request.EnrollmentYear,
            DateTime.UtcNow,
            request.IsPrimary,
            request.Notes);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "Student.ProgramEnrolled",
            "Academic",
            nameof(Student),
            student.Id.ToString(),
            newValues: new
            {
                EnrollmentId = enrollment.Id,
                request.DepartmentId,
                request.ProgramId,
                request.CurriculumVersionId,
                request.EnrollmentType,
                request.IsPrimary
            },
            cancellationToken: cancellationToken);

        return ServiceResult<Guid>.Created(enrollment.Id);
    }
}
