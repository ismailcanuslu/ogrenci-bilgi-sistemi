using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Programs;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;

namespace Academic.Application.Features.Programs;

public static class CreateProgram
{
    public sealed record Command(
        Guid DepartmentId,
        string Code,
        string Name,
        DegreeLevel DegreeLevel)
        : ICommand<ServiceResult<ProgramDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.DepartmentId).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(Program.CodeMaxLength);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(Program.NameMaxLength);
            RuleFor(x => x.DegreeLevel).IsInEnum();
        }
    }

    public sealed class Handler(
        AcademicAccess access,
        IDepartmentRepository departmentRepository,
        IProgramRepository programRepository,
        IAcademicUnitOfWork unitOfWork,
        IAuditService auditService)
        : ICommandHandler<Command, ServiceResult<ProgramDto>>
    {
        public async Task<ServiceResult<ProgramDto>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var department = await departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
            if (department is null)
            {
                return ServiceResult<ProgramDto>.NotFound(AcademicErrors.Department.NotFound(nameof(Command.DepartmentId)));
            }

            var denied = await access.CheckProgramCreationAsync(
                Permissions.Curriculum.Create,
                department.Id,
                department.FacultyId,
                cancellationToken);

            if (denied is not null)
            {
                return AuthorizationFailure.To<ProgramDto>(denied);
            }

            if (!department.IsActive)
            {
                return ServiceResult<ProgramDto>.Conflict(AcademicErrors.Faculty.Inactive);
            }

            var normalizedCode = Program.NormalizeCode(request.Code);
            if (await programRepository.CodeExistsInDepartmentAsync(department.Id, normalizedCode, null, cancellationToken))
            {
                return ServiceResult<ProgramDto>.Conflict(AcademicErrors.Program.CodeAlreadyExists);
            }

            var program = Program.Create(
                department.Id,
                request.Code,
                request.Name,
                request.DegreeLevel,
                DateTime.UtcNow);

            await programRepository.AddAsync(program, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await auditService.RecordAsync(
                "Program.Created", "Academic", "Program", program.Id.ToString(),
                newValues: new { program.DepartmentId, program.Code, program.Name, program.DegreeLevel },
                cancellationToken: cancellationToken);

            return ServiceResult<ProgramDto>.Created(ProgramDto.From(program));
        }
    }
}
