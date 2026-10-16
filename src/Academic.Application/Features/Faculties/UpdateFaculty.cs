using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Faculties;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Faculties;

public static class UpdateFaculty
{
    public sealed record Command(Guid Id, string Code, string Name)
        : ICommand<ServiceResult<FacultyDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(Faculty.CodeMaxLength);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(Faculty.NameMaxLength);
        }
    }

    public sealed class Handler(
        AcademicAccess access,
        IFacultyRepository facultyRepository,
        IAcademicUnitOfWork unitOfWork,
        IAuditService auditService)
        : ICommandHandler<Command, ServiceResult<FacultyDto>>
    {
        public async Task<ServiceResult<FacultyDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var faculty = await facultyRepository.GetByIdAsync(request.Id, cancellationToken);

            var denied = await access.CheckFacultyAsync(Permissions.Faculty.Update, request.Id, faculty is not null, cancellationToken);

            if (denied is not null)
            {
                return AuthorizationFailure.To<FacultyDto>(denied);
            }

            if (faculty is null)
            {
                return ServiceResult<FacultyDto>.NotFound(AcademicErrors.Faculty.NotFound());
            }

            if (!faculty.IsActive)
            {
                return ServiceResult<FacultyDto>.Conflict(AcademicErrors.Faculty.Inactive);
            }

            var newCode = Faculty.NormalizeCode(request.Code);

            if (await facultyRepository.CodeExistsAsync(newCode, faculty.Id, cancellationToken))
            {
                return ServiceResult<FacultyDto>.Conflict(AcademicErrors.Faculty.CodeAlreadyExists);
            }

            var oldValues = new { faculty.Code, faculty.Name };

            faculty.Update(request.Code, request.Name, DateTime.UtcNow);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                "Faculty.Updated", "Academic", "Faculty", faculty.Id.ToString(),
                oldValues: oldValues,
                newValues: new { faculty.Code, faculty.Name },
                cancellationToken: cancellationToken);

            return ServiceResult<FacultyDto>.Ok(FacultyDto.From(faculty));
        }
    }
}
