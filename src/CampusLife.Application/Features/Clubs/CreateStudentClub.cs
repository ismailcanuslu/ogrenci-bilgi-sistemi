using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using CampusLife.Application.Abstractions;
using CampusLife.Domain.Clubs;
using FluentValidation;
using MediatR;

namespace CampusLife.Application.Features.Clubs;

public sealed record CreateStudentClubCommand(
    string Name,
    string Code,
    string? Category,
    string? Description,
    Guid? AdvisorUserId,
    Guid? PresidentStudentId) : IRequest<ServiceResult<Guid>>;

public sealed class CreateStudentClubValidator : AbstractValidator<CreateStudentClubCommand>
{
    public CreateStudentClubValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(StudentClub.NameMaxLength);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(StudentClub.CodeMaxLength);
        RuleFor(x => x.Category).MaximumLength(StudentClub.CategoryMaxLength);
        RuleFor(x => x.Description).MaximumLength(StudentClub.DescriptionMaxLength);
    }
}

public sealed class CreateStudentClubHandler(
    IStudentClubRepository clubRepository,
    ICampusLifeUnitOfWork unitOfWork,
    IPermissionGuard permissionGuard,
    IAuditService auditService) : IRequestHandler<CreateStudentClubCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(CreateStudentClubCommand request, CancellationToken cancellationToken)
    {
        var authError = await permissionGuard.CheckAsync(Permissions.CampusLife.ClubManage, [ResourceScope.University], cancellationToken);
        if (authError is not null)
        {
            return AuthorizationFailure.To<Guid>(authError);
        }

        var existing = await clubRepository.GetByCodeAsync(request.Code, cancellationToken);
        if (existing is not null)
        {
            return ServiceResult<Guid>.Conflict(new Error("StudentClub.CodeAlreadyExists", ErrorType.Conflict));
        }

        var club = StudentClub.Create(
            request.Name,
            request.Code,
            request.Category,
            request.Description,
            request.AdvisorUserId,
            request.PresidentStudentId,
            DateTime.UtcNow);

        await clubRepository.AddClubAsync(club, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "CampusLife.ClubCreated",
            "CampusLife",
            nameof(StudentClub),
            club.Id.ToString(),
            newValues: new { club.Id, request.Name, request.Code },
            cancellationToken: cancellationToken);

        return ServiceResult<Guid>.Created(club.Id);
    }
}
