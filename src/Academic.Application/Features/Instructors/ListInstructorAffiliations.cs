using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Instructors;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;

namespace Academic.Application.Features.Instructors;

public sealed record InstructorAffiliationResponse(
    Guid Id,
    Guid InstructorId,
    Guid FacultyId,
    Guid DepartmentId,
    AffiliationType AffiliationType,
    string? Title,
    bool IsPrimary,
    DateTime StartDateUtc,
    DateTime? EndDateUtc,
    bool IsActive);

public sealed record ListInstructorAffiliationsQuery(Guid InstructorId) : IRequest<ServiceResult<IReadOnlyCollection<InstructorAffiliationResponse>>>;

public sealed class ListInstructorAffiliationsHandler(
    IInstructorAffiliationRepository affiliationRepository) : IRequestHandler<ListInstructorAffiliationsQuery, ServiceResult<IReadOnlyCollection<InstructorAffiliationResponse>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<InstructorAffiliationResponse>>> Handle(
        ListInstructorAffiliationsQuery request, CancellationToken cancellationToken)
    {
        var affiliations = await affiliationRepository.GetByInstructorIdAsync(request.InstructorId, cancellationToken);

        var dtos = affiliations.Select(a => new InstructorAffiliationResponse(
            a.Id,
            a.InstructorId,
            a.FacultyId,
            a.DepartmentId,
            a.AffiliationType,
            a.Title,
            a.IsPrimary,
            a.StartDateUtc,
            a.EndDateUtc,
            a.IsActive)).ToList();

        return ServiceResult<IReadOnlyCollection<InstructorAffiliationResponse>>.Ok(dtos);
    }
}
