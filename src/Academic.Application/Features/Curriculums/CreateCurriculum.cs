using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Curriculums;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Curriculums;

public static class CreateCurriculum
{
    public sealed record Command(
        Guid ProgramId,
        Guid AcademicYearId,
        string Code,
        string Name)
        : ICommand<ServiceResult<CurriculumDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ProgramId).NotEmpty();
            RuleFor(x => x.AcademicYearId).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(Curriculum.CodeMaxLength);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(Curriculum.NameMaxLength);
        }
    }

    public sealed class Handler(
        AcademicAccess access,
        IProgramRepository programRepository,
        IDepartmentRepository departmentRepository,
        IAcademicYearRepository yearRepository,
        ICurriculumRepository curriculumRepository,
        IAcademicUnitOfWork unitOfWork,
        IAuditService auditService)
        : ICommandHandler<Command, ServiceResult<CurriculumDto>>
    {
        public async Task<ServiceResult<CurriculumDto>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var program = await programRepository.GetByIdAsync(request.ProgramId, cancellationToken);
            if (program is null)
            {
                return ServiceResult<CurriculumDto>.NotFound(AcademicErrors.Program.NotFound(nameof(Command.ProgramId)));
            }

            var department = await departmentRepository.GetByIdAsync(program.DepartmentId, cancellationToken);
            if (department is null)
            {
                return ServiceResult<CurriculumDto>.NotFound(AcademicErrors.Department.NotFound());
            }

            var denied = await access.CheckCurriculumCreationAsync(
                Permissions.Curriculum.Create,
                department.Id,
                department.FacultyId,
                cancellationToken);

            if (denied is not null)
            {
                return AuthorizationFailure.To<CurriculumDto>(denied);
            }

            var year = await yearRepository.GetByIdAsync(request.AcademicYearId, cancellationToken);
            if (year is null)
            {
                return ServiceResult<CurriculumDto>.NotFound(AcademicErrors.AcademicYear.NotFound(nameof(Command.AcademicYearId)));
            }

            if (await curriculumRepository.ExistsForProgramAndYearAsync(program.Id, year.Id, null, cancellationToken))
            {
                return ServiceResult<CurriculumDto>.Conflict(AcademicErrors.Curriculum.AlreadyExistsForAcademicYear);
            }

            var curriculum = Curriculum.Create(
                program.Id,
                year.Id,
                request.Code,
                request.Name,
                DateTime.UtcNow);

            await curriculumRepository.AddAsync(curriculum, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                "Curriculum.Created", "Academic", "Curriculum", curriculum.Id.ToString(),
                newValues: new { curriculum.ProgramId, curriculum.AcademicYearId, curriculum.Code, curriculum.Name },
                cancellationToken: cancellationToken);

            return ServiceResult<CurriculumDto>.Created(CurriculumDto.From(curriculum));
        }
    }
}
