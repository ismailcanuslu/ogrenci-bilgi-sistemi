using BuildingBlocks.Application.Results;
using CampusLife.Application.Abstractions;
using MediatR;

namespace CampusLife.Application.Features.Clubs;

public sealed record StudentClubResponse(
    Guid Id,
    string Name,
    string Code,
    string? Category,
    string? Description,
    Guid? AdvisorUserId,
    Guid? PresidentStudentId,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record ListStudentClubsQuery(bool? ActiveOnly = true) : IRequest<ServiceResult<IReadOnlyCollection<StudentClubResponse>>>;

public sealed class ListStudentClubsHandler(
    IStudentClubRepository clubRepository) : IRequestHandler<ListStudentClubsQuery, ServiceResult<IReadOnlyCollection<StudentClubResponse>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<StudentClubResponse>>> Handle(
        ListStudentClubsQuery request, CancellationToken cancellationToken)
    {
        var clubs = await clubRepository.ListAsync(request.ActiveOnly, cancellationToken);
        var dtos = clubs.Select(c => new StudentClubResponse(
            c.Id,
            c.Name,
            c.Code,
            c.Category,
            c.Description,
            c.AdvisorUserId,
            c.PresidentStudentId,
            c.IsActive,
            c.CreatedAtUtc)).ToList();

        return ServiceResult<IReadOnlyCollection<StudentClubResponse>>.Ok(dtos);
    }
}
