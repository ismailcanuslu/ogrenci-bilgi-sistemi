using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Faculties;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Faculties;

public static class CreateFaculty
{
    public sealed record Command(string Code, string Name)
        : ICommand<ServiceResult<FacultyDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
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
            var denied = await access.CheckUniversityAsync(Permissions.Faculty.Create, cancellationToken);

            if (denied is not null)
            {
                return AuthorizationFailure.To<FacultyDto>(denied);
            }

            var faculty = Faculty.Create(request.Code, request.Name, DateTime.UtcNow);

            if (await facultyRepository.CodeExistsAsync(faculty.Code, null, cancellationToken))
            {
                return ServiceResult<FacultyDto>.Conflict(AcademicErrors.Faculty.CodeAlreadyExists);
            }

            await facultyRepository.AddAsync(faculty, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                "Faculty.Created", "Academic", "Faculty", faculty.Id.ToString(),
                newValues: new { faculty.Code, faculty.Name },
                cancellationToken: cancellationToken);

            return ServiceResult<FacultyDto>.Created(FacultyDto.From(faculty));
        }
    }
}
