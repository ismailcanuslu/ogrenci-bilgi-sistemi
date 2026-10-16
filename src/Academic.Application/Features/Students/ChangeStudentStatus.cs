using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Students;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.Students;

public sealed record ChangeStudentStatusCommand(
    Guid Id,
    StudentStatus Status,
    string? Reason) : IRequest<ServiceResult<Unit>>;

public sealed class ChangeStudentStatusValidator : AbstractValidator<ChangeStudentStatusCommand>
{
    public ChangeStudentStatusValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Student.IdRequired");
        RuleFor(x => x.Status).IsInEnum().WithMessage("Student.InvalidStatus");
        RuleFor(x => x.Reason).MaximumLength(500).WithMessage("Student.ReasonTooLong");
    }
}

public sealed class ChangeStudentStatusHandler(
    IStudentRepository studentRepository,
    IDepartmentRepository departmentRepository,
    IAcademicUnitOfWork unitOfWork,
    IAuditService auditService,
    AcademicAccess access) : IRequestHandler<ChangeStudentStatusCommand, ServiceResult<Unit>>
{
    public async Task<ServiceResult<Unit>> Handle(ChangeStudentStatusCommand request, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(request.Id, cancellationToken);
        var department = student is null
            ? null
            : await departmentRepository.GetByIdAsync(student.DepartmentId, cancellationToken);

        var authError = await access.CheckStudentAsync(
            Permissions.Student.EnrollmentManage,
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

        var previousStatus = student.Status;
        student.ChangeStatus(request.Status, DateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "Student.StatusChanged",
            "Academic",
            nameof(Student),
            student.Id.ToString(),
            newValues: new { PreviousStatus = previousStatus, NewStatus = request.Status, request.Reason },
            cancellationToken: cancellationToken);

        return ServiceResult<Unit>.Ok(Unit.Value);
    }
}
