using Auditing.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using CampusLife.Application.Abstractions;
using CampusLife.Domain.Clubs;
using FluentValidation;
using MediatR;

namespace CampusLife.Application.Features.Clubs;

public sealed record CreateClubEventCommand(
    Guid ClubId,
    string Title,
    string? Description,
    DateTime EventDateUtc,
    string Location,
    int Capacity) : IRequest<ServiceResult<Guid>>;

public sealed class CreateClubEventValidator : AbstractValidator<CreateClubEventCommand>
{
    public CreateClubEventValidator()
    {
        RuleFor(x => x.ClubId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(ClubEvent.TitleMaxLength);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(ClubEvent.LocationMaxLength);
        RuleFor(x => x.Capacity).GreaterThan(0);
    }
}

public sealed class CreateClubEventHandler(
    IStudentClubRepository clubRepository,
    ICampusLifeUnitOfWork unitOfWork,
    IPermissionGuard permissionGuard,
    IAuditService auditService) : IRequestHandler<CreateClubEventCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(CreateClubEventCommand request, CancellationToken cancellationToken)
    {
        var authError = await permissionGuard.CheckAsync(Permissions.CampusLife.EventManage, [ResourceScope.University], cancellationToken);
        if (authError is not null)
        {
            return AuthorizationFailure.To<Guid>(authError);
        }

        var club = await clubRepository.GetByIdAsync(request.ClubId, cancellationToken);
        if (club is null || !club.IsActive)
        {
            return ServiceResult<Guid>.NotFound(new Error("StudentClub.NotFoundOrInactive", ErrorType.NotFound));
        }

        var clubEvent = ClubEvent.Create(
            request.ClubId,
            request.Title,
            request.Description,
            request.EventDateUtc,
            request.Location,
            request.Capacity,
            DateTime.UtcNow);

        await clubRepository.AddEventAsync(clubEvent, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.RecordAsync(
            "CampusLife.EventCreated",
            "CampusLife",
            nameof(ClubEvent),
            clubEvent.Id.ToString(),
            newValues: new { clubEvent.Id, request.ClubId, request.Title, request.EventDateUtc },
            cancellationToken: cancellationToken);

        return ServiceResult<Guid>.Created(clubEvent.Id);
    }
}
