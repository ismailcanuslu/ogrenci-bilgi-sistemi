using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Curriculums;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Curriculums;

public static class PublishCurriculumVersion
{
    public sealed record Command(Guid CurriculumId, Guid VersionId)
        : ICommand<ServiceResult<CurriculumVersionDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CurriculumId).NotEmpty();
            RuleFor(x => x.VersionId).NotEmpty();
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

            // Yayınlama işlemi özel olarak curriculum.publish yetkisi gerektirir.
            var denied = await access.CheckCurriculumAsync(
                Permissions.Curriculum.Publish,
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

            var version = curriculum.Versions.FirstOrDefault(v => v.Id == request.VersionId);
            if (version is null)
            {
                return ServiceResult<CurriculumVersionDto>.NotFound(AcademicErrors.Curriculum.VersionNotFound);
            }

            if (version.Status != CurriculumVersionStatus.Draft)
            {
                return ServiceResult<CurriculumVersionDto>.Conflict(AcademicErrors.Curriculum.VersionNotDraft);
            }

            curriculum.PublishVersion(version.Id, DateTime.UtcNow);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                "CurriculumVersion.Published", "Academic", "CurriculumVersion", version.Id.ToString(),
                newValues: new { version.CurriculumId, version.VersionNumber, version.Status, version.PublishedAtUtc },
                cancellationToken: cancellationToken);

            return ServiceResult<CurriculumVersionDto>.Ok(CurriculumVersionDto.From(version));
        }
    }
}
