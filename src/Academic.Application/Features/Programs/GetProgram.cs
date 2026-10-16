using Academic.Application.Abstractions;
using Academic.Application.Common;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Programs;

public static class GetProgram
{
    public sealed record Query(Guid Id) : IQuery<ServiceResult<ProgramDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator() => RuleFor(x => x.Id).NotEmpty();
    }

    public sealed class Handler(
        AcademicAccess access,
        IProgramRepository programRepository,
        IDepartmentRepository departmentRepository)
        : IQueryHandler<Query, ServiceResult<ProgramDto>>
    {
        public async Task<ServiceResult<ProgramDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var program = await programRepository.GetByIdAsync(request.Id, cancellationToken);
            var department = program is not null
                ? await departmentRepository.GetByIdAsync(program.DepartmentId, cancellationToken)
                : null;

            var denied = await access.CheckProgramAsync(
                Permissions.Curriculum.Read,
                department?.Id,
                department?.FacultyId,
                program is not null,
                cancellationToken);

            if (denied is not null)
            {
                return AuthorizationFailure.To<ProgramDto>(denied);
            }

            return program is null
                ? ServiceResult<ProgramDto>.NotFound(AcademicErrors.Program.NotFound())
                : ServiceResult<ProgramDto>.Ok(ProgramDto.From(program));
        }
    }
}
