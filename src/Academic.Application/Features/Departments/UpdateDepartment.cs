using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Departments;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Departments;

public static class UpdateDepartment
{
    public const string DepartmentInactiveCode = "Department.Inactive";

    public sealed record Command(Guid Id, string Code, string Name)
        : ICommand<ServiceResult<DepartmentDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(Department.CodeMaxLength);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(Department.NameMaxLength);
        }
    }

    public sealed class Handler(
        AcademicAccess access,
        IDepartmentRepository departmentRepository,
        IAcademicUnitOfWork unitOfWork,
        IAuditService auditService)
        : ICommandHandler<Command, ServiceResult<DepartmentDto>>
    {
        public async Task<ServiceResult<DepartmentDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var department = await departmentRepository.GetByIdAsync(request.Id, cancellationToken);

            var denied = await access.CheckDepartmentAsync(Permissions.Department.Update, department, cancellationToken);

            if (denied is not null)
            {
                return AuthorizationFailure.To<DepartmentDto>(denied);
            }

            if (department is null)
            {
                return ServiceResult<DepartmentDto>.NotFound(AcademicErrors.Department.NotFound());
            }

            if (!department.IsActive)
            {
                return ServiceResult<DepartmentDto>.Conflict(
                    new Error(DepartmentInactiveCode, ErrorType.Conflict));
            }

            var newCode = Department.NormalizeCode(request.Code);

            if (await departmentRepository.CodeExistsInFacultyAsync(
                    department.FacultyId, newCode, department.Id, cancellationToken))
            {
                return ServiceResult<DepartmentDto>.Conflict(AcademicErrors.Department.CodeAlreadyExists);
            }

            var oldValues = new { department.Code, department.Name };

            department.Update(request.Code, request.Name, DateTime.UtcNow);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                "Department.Updated", "Academic", "Department", department.Id.ToString(),
                oldValues: oldValues,
                newValues: new { department.Code, department.Name },
                cancellationToken: cancellationToken);

            return ServiceResult<DepartmentDto>.Ok(DepartmentDto.From(department));
        }
    }
}
