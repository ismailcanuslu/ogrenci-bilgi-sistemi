using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Programs;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Programs;

public static class UpdateProgram
{
    public sealed record Command(
        Guid Id,
        string Code,
        string Name,
        DegreeLevel DegreeLevel)
        : ICommand<ServiceResult<ProgramDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(Program.CodeMaxLength);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(Program.NameMaxLength);
            RuleFor(x => x.DegreeLevel).IsInEnum();
        }
    }

    public sealed class Handler(
        AcademicAccess access,
        IProgramRepository programRepository,
        IDepartmentRepository departmentRepository,
        IAcademicUnitOfWork unitOfWork,
        IAuditService auditService)
        : ICommandHandler<Command, ServiceResult<ProgramDto>>
    {
        public async Task<ServiceResult<ProgramDto>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var program = await programRepository.GetByIdAsync(request.Id, cancellationToken);
            var department = program is not null
                ? await departmentRepository.GetByIdAsync(program.DepartmentId, cancellationToken)
                : null;

            var denied = await access.CheckProgramAsync(
                Permissions.Curriculum.Update,
                department?.Id,
                department?.FacultyId,
                program is not null,
                cancellationToken);

            if (denied is not null)
            {
                return AuthorizationFailure.To<ProgramDto>(denied);
            }

            if (program is null)
            {
                return ServiceResult<ProgramDto>.NotFound(AcademicErrors.Program.NotFound());
            }

            if (!program.IsActive)
            {
                return ServiceResult<ProgramDto>.Conflict(AcademicErrors.Program.Inactive);
            }

            var normalizedCode = Program.NormalizeCode(request.Code);
            if (await programRepository.CodeExistsInDepartmentAsync(program.DepartmentId, normalizedCode, program.Id, cancellationToken))
            {
                return ServiceResult<ProgramDto>.Conflict(AcademicErrors.Program.CodeAlreadyExists);
            }

            var oldValues = new { program.Code, program.Name, program.DegreeLevel };

            program.Update(request.Code, request.Name, request.DegreeLevel, DateTime.UtcNow);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                "Program.Updated", "Academic", "Program", program.Id.ToString(),
                oldValues: oldValues,
                newValues: new { program.Code, program.Name, program.DegreeLevel },
                cancellationToken: cancellationToken);

            return ServiceResult<ProgramDto>.Ok(ProgramDto.From(program));
        }
    }
}
