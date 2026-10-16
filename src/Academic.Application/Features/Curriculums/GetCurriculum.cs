using Academic.Application.Abstractions;
using Academic.Application.Common;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Curriculums;

public static class GetCurriculum
{
    public sealed record Query(Guid Id) : IQuery<ServiceResult<CurriculumDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.Id).NotEmpty();
    }

    public sealed class Handler(
        AcademicAccess access,
        ICurriculumRepository curriculumRepository,
        IProgramRepository programRepository,
        IDepartmentRepository departmentRepository)
        : IQueryHandler<Query, ServiceResult<CurriculumDto>>
    {
        public async Task<ServiceResult<CurriculumDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var curriculum = await curriculumRepository.GetByIdAsync(request.Id, cancellationToken);
            var program = curriculum is not null
                ? await programRepository.GetByIdAsync(curriculum.ProgramId, cancellationToken)
                : null;
            var department = program is not null
                ? await departmentRepository.GetByIdAsync(program.DepartmentId, cancellationToken)
                : null;

            var denied = await access.CheckCurriculumAsync(
                Permissions.Curriculum.Read,
                department?.Id,
                department?.FacultyId,
                curriculum is not null,
                cancellationToken);

            if (denied is not null)
            {
                return AuthorizationFailure.To<CurriculumDto>(denied);
            }

            return curriculum is null
                ? ServiceResult<CurriculumDto>.NotFound(AcademicErrors.Curriculum.NotFound())
                : ServiceResult<CurriculumDto>.Ok(CurriculumDto.From(curriculum));
        }
    }
}
