using Academic.Domain.Courses;
using Academic.Application.Common;
using BuildingBlocks.Application.Results;

namespace Academic.Application.Abstractions;

public sealed record CourseListCriteria(
    IReadOnlyCollection<Guid>? DepartmentIds,
    bool? IsActive,
    string? Search,
    int Page,
    int PageSize);

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Course?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(string code, Guid? excludeId, CancellationToken cancellationToken = default);

    Task AddAsync(Course course, CancellationToken cancellationToken = default);

    Task<PagedResult<Course>> ListAsync(CourseListCriteria criteria, CancellationToken cancellationToken = default);
}
