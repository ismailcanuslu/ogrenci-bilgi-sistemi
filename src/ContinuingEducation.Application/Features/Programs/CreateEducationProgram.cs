using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using ContinuingEducation.Application.Abstractions;
using ContinuingEducation.Domain.Programs;
using FluentValidation;
using MediatR;

namespace ContinuingEducation.Application.Features.Programs;

public sealed record CreateEducationProgramCommand(
    string Title,
    string Code,
    string? Description,
    Guid InstructorUserId,
    int Capacity,
    DateTime StartDateUtc,
    DateTime EndDateUtc,
    int TotalHours,
    decimal Price,
    bool IsFree,
    int MinAttendancePercentage = 70,
    decimal MinPassingGrade = 60) : IRequest<ServiceResult<Guid>>;

public sealed class CreateEducationProgramValidator : AbstractValidator<CreateEducationProgramCommand>
{
    public CreateEducationProgramValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(EducationProgram.TitleMaxLength);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(EducationProgram.CodeMaxLength);
        RuleFor(x => x.InstructorUserId).NotEmpty();
        RuleFor(x => x.Capacity).GreaterThan(0);
        RuleFor(x => x.StartDateUtc).LessThan(x => x.EndDateUtc);
        RuleFor(x => x.TotalHours).GreaterThan(0);
        RuleFor(x => x.MinAttendancePercentage).InclusiveBetween(0, 100);
        RuleFor(x => x.MinPassingGrade).InclusiveBetween(0, 100);
    }
}

public sealed class CreateEducationProgramHandler(
    IEducationProgramRepository programRepository,
    IContinuingEducationUnitOfWork unitOfWork,
    IPermissionGuard permissionGuard,
    IAuditService auditService) : IRequestHandler<CreateEducationProgramCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(CreateEducationProgramCommand request, CancellationToken cancellationToken)
    {
        var authError = await permissionGuard.CheckAsync(Permissions.ContinuingEducation.ProgramManage, [ResourceScope.University], cancellationToken);
        if (authError is not null)
        {
            return AuthorizationFailure.To<Guid>(authError);
        }

        var existing = await programRepository.GetByCodeAsync(request.Code, cancellationToken);
        if (existing is not null)
        {
            return ServiceResult<Guid>.Conflict(new Error("EducationProgram.CodeAlreadyExists", ErrorType.Conflict));
        }

        var program = EducationProgram.Create(
            request.Title,
            request.Code,
            request.Description,
            request.InstructorUserId,
            request.Capacity,
            request.StartDateUtc,
            request.EndDateUtc,
            request.TotalHours,
            request.Price,
            request.IsFree,
            request.MinAttendancePercentage,
            request.MinPassingGrade,
            DateTime.UtcNow);

        await programRepository.AddAsync(program, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "ContinuingEducation.ProgramCreated",
            "ContinuingEducation",
            nameof(EducationProgram),
            program.Id.ToString(),
            newValues: new { program.Id, request.Code, request.Title, request.TotalHours },
            cancellationToken: cancellationToken);

        return ServiceResult<Guid>.Created(program.Id);
    }
}
