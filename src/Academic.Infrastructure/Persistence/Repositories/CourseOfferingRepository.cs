using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Courses;
using Microsoft.EntityFrameworkCore;

namespace Academic.Infrastructure.Persistence.Repositories;

public sealed class CourseOfferingRepository(AcademicDbContext dbContext) : ICourseOfferingRepository
{
    public Task<CourseOffering?> GetByIdAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = dbContext.CourseOfferings.AsQueryable();
        if (includeDetails)
        {
            query = query
                .Include(x => x.Instructors)
                .Include(x => x.Schedules);
        }

        return query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> SectionExistsAsync(
        Guid courseId, Guid academicTermId, int section, Guid? excludeId, CancellationToken cancellationToken = default)
        => dbContext.CourseOfferings.AnyAsync(
            x => x.CourseId == courseId &&
                 x.AcademicTermId == academicTermId &&
                 x.Section == section &&
                 (excludeId == null || x.Id != excludeId),
            cancellationToken);

    public Task<bool> HasInstructorConflictAsync(
        Guid instructorId,
        Guid academicTermId,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeOfferingId,
        CancellationToken cancellationToken = default)
    {
        // Eğitmenin oturum bazlı (schedule.InstructorId) veya ders bazlı (offering.Instructors) olarak o saatte başka dersi var mı?
        return (from s in dbContext.CourseOfferingSchedules
                join o in dbContext.CourseOfferings on s.CourseOfferingId equals o.Id
                where o.AcademicTermId == academicTermId &&
                      (excludeOfferingId == null || o.Id != excludeOfferingId) &&
                      s.DayOfWeek == dayOfWeek &&
                      s.StartTime < endTime &&
                      s.EndTime > startTime &&
                      (s.InstructorId == instructorId ||
                       (s.InstructorId == null && o.Instructors.Any(i => i.InstructorId == instructorId)))
                select s.Id).AnyAsync(cancellationToken);
    }

    public Task<bool> HasClassroomConflictAsync(
        string buildingOrFaculty,
        string classroom,
        Guid academicTermId,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeOfferingId,
        CancellationToken cancellationToken = default)
    {
        var normalizedBuilding = buildingOrFaculty.Trim().ToLower();
        var normalizedClassroom = classroom.Trim().ToLower();

        return (from s in dbContext.CourseOfferingSchedules
                join o in dbContext.CourseOfferings on s.CourseOfferingId equals o.Id
                where !s.IsOnline &&
                      o.AcademicTermId == academicTermId &&
                      s.BuildingOrFaculty.ToLower() == normalizedBuilding &&
                      s.Classroom.ToLower() == normalizedClassroom &&
                      (excludeOfferingId == null || o.Id != excludeOfferingId) &&
                      s.DayOfWeek == dayOfWeek &&
                      s.StartTime < endTime &&
                      s.EndTime > startTime
                select s.Id).AnyAsync(cancellationToken);
    }

    public async Task AddAsync(CourseOffering offering, CancellationToken cancellationToken = default)
        => await dbContext.CourseOfferings.AddAsync(offering, cancellationToken);

    public async Task<PagedResult<CourseOffering>> ListAsync(CourseOfferingListCriteria criteria, CancellationToken cancellationToken = default)
    {
        var query = dbContext.CourseOfferings
            .Include(x => x.Instructors)
            .Include(x => x.Schedules)
            .AsNoTracking()
            .AsQueryable();

        if (criteria.AcademicTermId is { } termId)
        {
            query = query.Where(x => x.AcademicTermId == termId);
        }

        if (criteria.CourseId is { } courseId)
        {
            query = query.Where(x => x.CourseId == courseId);
        }

        if (criteria.InstructorId is { } instructorId)
        {
            query = query.Where(x =>
                x.Instructors.Any(i => i.InstructorId == instructorId) ||
                x.Schedules.Any(s => s.InstructorId == instructorId));
        }

        if (criteria.IsRegistrationOpen is { } isOpen)
        {
            query = query.Where(x => x.IsRegistrationOpen == isOpen);
        }

        if (criteria.DepartmentIds is { Count: > 0 } deptIds)
        {
            query = from o in query
                    join c in dbContext.Courses on o.CourseId equals c.Id
                    where deptIds.Contains(c.DepartmentId)
                    select o;
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Section)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<CourseOffering>(items, criteria.Page, criteria.PageSize, total);
    }
}
