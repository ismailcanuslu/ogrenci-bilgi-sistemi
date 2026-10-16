using BuildingBlocks.Application.Results;
using CampusLife.Application.Abstractions;
using CampusLife.Domain.Clubs;
using MediatR;

namespace CampusLife.Application.Features.Clubs;

public sealed record ClubEventResponse(
    Guid Id,
    Guid ClubId,
    string Title,
    string? Description,
    DateTime EventDateUtc,
    string Location,
    int Capacity,
    EventStatus Status,
    Guid? ReviewedByUserId,
    string? ReviewNotes,
    DateTime CreatedAtUtc,
    DateTime? ReviewedAtUtc);

public sealed record ListClubEventsQuery(
    Guid? ClubId = null,
    EventStatus? Status = null) : IRequest<ServiceResult<IReadOnlyCollection<ClubEventResponse>>>;

public sealed class ListClubEventsHandler(
    IStudentClubRepository clubRepository) : IRequestHandler<ListClubEventsQuery, ServiceResult<IReadOnlyCollection<ClubEventResponse>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<ClubEventResponse>>> Handle(
        ListClubEventsQuery request, CancellationToken cancellationToken)
    {
        var events = await clubRepository.ListEventsAsync(request.ClubId, request.Status, cancellationToken);
        var dtos = events.Select(e => new ClubEventResponse(
            e.Id,
            e.ClubId,
            e.Title,
            e.Description,
            e.EventDateUtc,
            e.Location,
            e.Capacity,
            e.Status,
            e.ReviewedByUserId,
            e.ReviewNotes,
            e.CreatedAtUtc,
            e.ReviewedAtUtc)).ToList();

        return ServiceResult<IReadOnlyCollection<ClubEventResponse>>.Ok(dtos);
    }
}
