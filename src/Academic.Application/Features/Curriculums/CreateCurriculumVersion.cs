using Academic.Application.Abstractions;
using Academic.Application.Common;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Curriculums;

public static class CreateCurriculumVersion
{
    public sealed record Command(Guid CurriculumId, string? Notes)
        : ICommand<ServiceResult<CurriculumVersionDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CurriculumId).NotEmpty();
            RuleFor(x => x.Notes).MaximumLength(500);
        }
    }

    public sealed class Handler(
        AcademicAccess access,
        ICurriculumRepository curriculumRepository,
        IProgramRepository programRepository,
        IDepartmentRepository departmentRepository,
        IAcademicUnitOfWork unitOfWork,
        IAuditService auditService)
        : ICommandHandler<Command, ServiceResult<CurriculumVersionDto>>
    {
        public async Task<ServiceResult<CurriculumVersionDto>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var curriculum = await curriculumRepository.GetByIdAsync(request.CurriculumId, cancellationToken);
            var program = curriculum is not null
                ? await programRepository.GetByIdAsync(curriculum.ProgramId, cancellationToken)
                : null;
            var department = program is not null
                ? await departmentRepository.GetByIdAsync(program.DepartmentId, cancellationToken)
                : null;

            var denied = await access.CheckCurriculumAsync(
                Permissions.Curriculum.Update,
                department?.Id,
                department?.FacultyId,
                curriculum is not null,
                cancellationToken);

            if (denied is not null)
            {
                return AuthorizationFailure.To<CurriculumVersionDto>(denied);
            }

            if (curriculum is null)
            {
                return ServiceResult<CurriculumVersionDto>.NotFound(AcademicErrors.Curriculum.NotFound());
            }

            if (!curriculum.IsActive)
            {
                return ServiceResult<CurriculumVersionDto>.Conflict(AcademicErrors.Curriculum.Inactive);
            }

            var newVersion = curriculum.CreateNewVersion(request.Notes, DateTime.UtcNow);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                "CurriculumVersion.Created", "Academic", "CurriculumVersion", newVersion.Id.ToString(),
                newValues: new { newVersion.CurriculumId, newVersion.VersionNumber, newVersion.Notes },
                cancellationToken: cancellationToken);

            return ServiceResult<CurriculumVersionDto>.Created(CurriculumVersionDto.From(newVersion));
        }
    }
}
