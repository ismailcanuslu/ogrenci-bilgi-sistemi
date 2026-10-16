using Academic.Application.Common;
using Academic.Domain.Faculties;

namespace Academic.Application.Abstractions;

public sealed record FacultyListCriteria(
    IReadOnlyCollection<Guid>? AllowedFacultyIds,
    string? Search,
    bool? IsActive,
    string SortBy,
    SortDirection SortDirection,
    int Page,
    int PageSize);

public interface IFacultyRepository
{
    Task<Faculty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(string code, Guid? excludeId, CancellationToken cancellationToken = default);

    Task AddAsync(Faculty faculty, CancellationToken cancellationToken = default);

    Task<PagedResult<Faculty>> ListAsync(FacultyListCriteria criteria, CancellationToken cancellationToken = default);
}
