using Academic.Domain.Courses;
using Academic.Application.Common;
using BuildingBlocks.Application.Results;

namespace Academic.Application.Abstractions;

public sealed record CourseOfferingListCriteria(
    Guid? AcademicTermId,
    Guid? CourseId,
    Guid? InstructorId,
    IReadOnlyCollection<Guid>? DepartmentIds,
    bool? IsRegistrationOpen,
    int Page,
    int PageSize);

public interface ICourseOfferingRepository
{
    Task<CourseOffering?> GetByIdAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default);

    Task<bool> SectionExistsAsync(Guid courseId, Guid academicTermId, int section, Guid? excludeId, CancellationToken cancellationToken = default);

    Task<bool> HasInstructorConflictAsync(
        Guid instructorId,
        Guid academicTermId,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeOfferingId,
        CancellationToken cancellationToken = default);

    Task<bool> HasClassroomConflictAsync(
        string buildingOrFaculty,
        string classroom,
        Guid academicTermId,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeOfferingId,
        CancellationToken cancellationToken = default);

    Task AddAsync(CourseOffering offering, CancellationToken cancellationToken = default);

    Task<PagedResult<CourseOffering>> ListAsync(CourseOfferingListCriteria criteria, CancellationToken cancellationToken = default);
}
