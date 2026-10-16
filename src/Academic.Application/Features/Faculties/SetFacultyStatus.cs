using Academic.Application.Abstractions;
using Academic.Application.Common;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Faculties;

/// <summary>Fakülteyi pasifleştirir/yeniden etkinleştirir (fiziksel silme yoktur).</summary>
public static class SetFacultyStatus
{
    public sealed record Command(Guid Id, bool IsActive)
        : ICommand<ServiceResult<FacultyDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.Id).NotEmpty();
    }

    public sealed class Handler(
        AcademicAccess access,
        IFacultyRepository facultyRepository,
        IDepartmentRepository departmentRepository,
        IAcademicUnitOfWork unitOfWork,
        IAuditService auditService)
        : ICommandHandler<Command, ServiceResult<FacultyDto>>
    {
        public async Task<ServiceResult<FacultyDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var faculty = await facultyRepository.GetByIdAsync(request.Id, cancellationToken);

            var denied = await access.CheckFacultyAsync(Permissions.Faculty.Deactivate, request.Id, faculty is not null, cancellationToken);

            if (denied is not null)
            {
                return AuthorizationFailure.To<FacultyDto>(denied);
            }

            if (faculty is null)
            {
                return ServiceResult<FacultyDto>.NotFound(AcademicErrors.Faculty.NotFound());
            }

            if (faculty.IsActive == request.IsActive)
            {
                return ServiceResult<FacultyDto>.Ok(FacultyDto.From(faculty));
            }

            var wasActive = faculty.IsActive;

            if (!request.IsActive)
            {
                if (await departmentRepository.HasActiveDepartmentsAsync(faculty.Id, cancellationToken))
                {
                    return ServiceResult<FacultyDto>.Conflict(AcademicErrors.Faculty.HasActiveDepartments);
                }

                faculty.Deactivate(DateTime.UtcNow);
            }
            else
            {
                faculty.Activate(DateTime.UtcNow);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                request.IsActive ? "Faculty.Activated" : "Faculty.Deactivated",
                "Academic", "Faculty", faculty.Id.ToString(),
                oldValues: new { IsActive = wasActive },
                newValues: new { faculty.IsActive },
                cancellationToken: cancellationToken);

            return ServiceResult<FacultyDto>.Ok(FacultyDto.From(faculty));
        }
    }
}
