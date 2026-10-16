using BuildingBlocks.Application.Results;
using ContinuingEducation.Application.Abstractions;
using ContinuingEducation.Domain.Programs;
using MediatR;

namespace ContinuingEducation.Application.Features.Programs;

public sealed record ListEducationProgramsQuery(
    CourseProgramStatus? Status = null) : IRequest<ServiceResult<IReadOnlyCollection<EducationProgramDto>>>;

public sealed record EducationProgramDto(
    Guid Id,
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
    int MinAttendancePercentage,
    decimal MinPassingGrade,
    string Status,
    DateTime CreatedAtUtc);

public sealed class ListEducationProgramsHandler(
    IEducationProgramRepository repository) : IRequestHandler<ListEducationProgramsQuery, ServiceResult<IReadOnlyCollection<EducationProgramDto>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<EducationProgramDto>>> Handle(
        ListEducationProgramsQuery request,
        CancellationToken cancellationToken)
    {
        var programs = await repository.ListAsync(request.Status, cancellationToken);

        var dtos = programs.Select(p => new EducationProgramDto(
            p.Id,
            p.Title,
            p.Code,
            p.Description,
            p.InstructorUserId,
            p.Capacity,
            p.StartDateUtc,
            p.EndDateUtc,
            p.TotalHours,
            p.Price,
            p.IsFree,
            p.MinAttendancePercentage,
            p.MinPassingGrade,
            p.Status.ToString(),
            p.CreatedAtUtc
        )).ToList();

        return ServiceResult<IReadOnlyCollection<EducationProgramDto>>.Ok(dtos);
    }
}
