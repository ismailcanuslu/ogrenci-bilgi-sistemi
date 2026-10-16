using Academic.Application.Abstractions;
using Academic.Application.Common;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Departments;

public static class SetDepartmentStatus
{
    public sealed record Command(Guid Id, bool IsActive)
        : ICommand<ServiceResult<DepartmentDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.Id).NotEmpty();
    }

    public sealed class Handler(
        AcademicAccess access,
        IDepartmentRepository departmentRepository,
        IFacultyRepository facultyRepository,
        IAcademicUnitOfWork unitOfWork,
        IAuditService auditService)
        : ICommandHandler<Command, ServiceResult<DepartmentDto>>
    {
        public async Task<ServiceResult<DepartmentDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var department = await departmentRepository.GetByIdAsync(request.Id, cancellationToken);

            var denied = await access.CheckDepartmentAsync(Permissions.Department.Deactivate, department, cancellationToken);

            if (denied is not null)
            {
                return AuthorizationFailure.To<DepartmentDto>(denied);
            }

            if (department is null)
            {
                return ServiceResult<DepartmentDto>.NotFound(AcademicErrors.Department.NotFound());
            }

            if (department.IsActive == request.IsActive)
            {
                return ServiceResult<DepartmentDto>.Ok(DepartmentDto.From(department));
            }

            var wasActive = department.IsActive;

            if (request.IsActive)
            {
                var faculty = await facultyRepository.GetByIdAsync(department.FacultyId, cancellationToken);

                if (faculty is null || !faculty.IsActive)
                {
                    return ServiceResult<DepartmentDto>.Conflict(AcademicErrors.Faculty.Inactive);
                }

                department.Activate(DateTime.UtcNow);
            }
            else
            {
                department.Deactivate(DateTime.UtcNow);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                request.IsActive ? "Department.Activated" : "Department.Deactivated",
                "Academic", "Department", department.Id.ToString(),
                oldValues: new { IsActive = wasActive },
                newValues: new { department.IsActive },
                cancellationToken: cancellationToken);

            return ServiceResult<DepartmentDto>.Ok(DepartmentDto.From(department));
        }
    }
}
