using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Departments;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Departments;

public static class CreateDepartment
{
    public sealed record Command(Guid FacultyId, string Code, string Name)
        : ICommand<ServiceResult<DepartmentDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FacultyId).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(Department.CodeMaxLength);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(Department.NameMaxLength);
        }
    }

    public sealed class Handler(
        AcademicAccess access,
        IFacultyRepository facultyRepository,
        IDepartmentRepository departmentRepository,
        IAcademicUnitOfWork unitOfWork,
        IAuditService auditService)
        : ICommandHandler<Command, ServiceResult<DepartmentDto>>
    {
        public async Task<ServiceResult<DepartmentDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            // Hedef fakülte kapsamında create izni gerekir; request gövdesindeki FacultyId ile
            // başka fakülteye bölüm eklenemez.
            var denied = await access.CheckDepartmentCreationAsync(
                Permissions.Department.Create, request.FacultyId, cancellationToken);

            if (denied is not null)
            {
                return AuthorizationFailure.To<DepartmentDto>(denied);
            }

            var faculty = await facultyRepository.GetByIdAsync(request.FacultyId, cancellationToken);

            if (faculty is null)
            {
                return ServiceResult<DepartmentDto>.NotFound(AcademicErrors.Faculty.NotFound(nameof(Command.FacultyId)));
            }

            if (!faculty.IsActive)
            {
                return ServiceResult<DepartmentDto>.Conflict(AcademicErrors.Faculty.Inactive);
            }

            var department = Department.Create(faculty.Id, request.Code, request.Name, DateTime.UtcNow);

            if (await departmentRepository.CodeExistsInFacultyAsync(faculty.Id, department.Code, null, cancellationToken))
            {
                return ServiceResult<DepartmentDto>.Conflict(AcademicErrors.Department.CodeAlreadyExists);
            }

            await departmentRepository.AddAsync(department, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                "Department.Created", "Academic", "Department", department.Id.ToString(),
                newValues: new { department.FacultyId, department.Code, department.Name },
                cancellationToken: cancellationToken);

            return ServiceResult<DepartmentDto>.Created(DepartmentDto.From(department));
        }
    }
}
