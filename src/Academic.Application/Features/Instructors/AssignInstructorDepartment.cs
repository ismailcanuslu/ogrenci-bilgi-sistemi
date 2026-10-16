using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Instructors;
using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;

namespace Academic.Application.Features.Instructors;

public sealed record AssignInstructorDepartmentCommand(
    Guid InstructorId,
    Guid FacultyId,
    Guid DepartmentId,
    AffiliationType AffiliationType,
    string? Title,
    bool IsPrimary,
    DateTime StartDateUtc,
    DateTime? EndDateUtc = null) : IRequest<ServiceResult<Guid>>;

public sealed class AssignInstructorDepartmentValidator : AbstractValidator<AssignInstructorDepartmentCommand>
{
    public AssignInstructorDepartmentValidator()
    {
        RuleFor(x => x.InstructorId).NotEmpty().WithMessage("InstructorAffiliation.InstructorRequired");
        RuleFor(x => x.FacultyId).NotEmpty().WithMessage("InstructorAffiliation.FacultyRequired");
        RuleFor(x => x.DepartmentId).NotEmpty().WithMessage("InstructorAffiliation.DepartmentRequired");
        RuleFor(x => x.AffiliationType).IsInEnum().WithMessage("InstructorAffiliation.InvalidAffiliationType");
    }
}

public sealed class AssignInstructorDepartmentHandler(
    IInstructorAffiliationRepository affiliationRepository,
    IDepartmentRepository departmentRepository,
    IAcademicUnitOfWork unitOfWork,
    IAuditService auditService,
    AcademicAccess access) : IRequestHandler<AssignInstructorDepartmentCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(AssignInstructorDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
        var authError = await access.CheckDepartmentAsync(
            Permissions.Course.AssignmentManage,
            department,
            cancellationToken);

        if (authError is not null)
        {
            return AuthorizationFailure.To<Guid>(authError);
        }

        if (department is null)
        {
            return ServiceResult<Guid>.NotFound(AcademicErrors.Department.NotFound());
        }

        if (request.IsPrimary)
        {
            var existingAffiliations = await affiliationRepository.GetByInstructorIdAsync(request.InstructorId, cancellationToken);
            foreach (var existing in existingAffiliations.Where(a => a.IsPrimary))
            {
                existing.SetPrimary(false, DateTime.UtcNow);
            }
        }

        var affiliation = InstructorDepartmentAffiliation.Create(
            request.InstructorId,
            request.FacultyId,
            request.DepartmentId,
            request.AffiliationType,
            request.Title,
            request.IsPrimary,
            request.StartDateUtc,
            request.EndDateUtc,
            DateTime.UtcNow);

        await affiliationRepository.AddAsync(affiliation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "Instructor.AffiliationAssigned",
            "Academic",
            nameof(InstructorDepartmentAffiliation),
            affiliation.Id.ToString(),
            newValues: new
            {
                affiliation.Id,
                request.InstructorId,
                request.FacultyId,
                request.DepartmentId,
                request.AffiliationType,
                request.Title,
                request.IsPrimary
            },
            cancellationToken: cancellationToken);

        return ServiceResult<Guid>.Created(affiliation.Id);
    }
}
