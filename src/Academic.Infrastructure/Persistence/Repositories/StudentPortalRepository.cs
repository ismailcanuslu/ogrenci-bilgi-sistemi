using Academic.Application.Abstractions;
using Academic.Domain.AcademicCalendars;
using Academic.Domain.Courses;
using Academic.Domain.Students;
using Microsoft.EntityFrameworkCore;

namespace Academic.Infrastructure.Persistence.Repositories;

public sealed class StudentPortalRepository : IStudentPortalRepository
{
    private readonly AcademicDbContext _dbContext;

    public StudentPortalRepository(AcademicDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Student?> GetStudentByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Students
            .Include(s => s.ProgramEnrollments)
            .FirstOrDefaultAsync(s => s.UserId == userId || s.Id == userId, cancellationToken);
    }

    public Task<Student?> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Students
            .Include(s => s.ProgramEnrollments)
            .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<StudentTuition>> GetTuitionsAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.StudentTuitions
            .Where(t => t.StudentId == studentId)
            .OrderByDescending(t => t.DueDateUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<StudentMilitaryStatus?> GetMilitaryStatusAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return _dbContext.StudentMilitaryStatuses
            .FirstOrDefaultAsync(m => m.StudentId == studentId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<StudentDisciplinaryRecord>> GetDisciplinaryRecordsAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.StudentDisciplinaryRecords
            .Where(d => d.StudentId == studentId)
            .OrderByDescending(d => d.DecisionDate)
            .ToListAsync(cancellationToken);
    }

    public Task<StudentBankDetails?> GetBankDetailsAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return _dbContext.StudentBankDetails
            .FirstOrDefaultAsync(b => b.StudentId == studentId, cancellationToken);
    }

    public async Task SaveBankDetailsAsync(StudentBankDetails details, CancellationToken cancellationToken = default)
    {
        var existing = await GetBankDetailsAsync(details.StudentId, cancellationToken);
        if (existing is null)
        {
            await _dbContext.StudentBankDetails.AddAsync(details, cancellationToken);
        }
        else
        {
            existing.Update(details.BankName, details.Iban, details.AccountHolderName, DateTime.UtcNow);
        }
    }

    public Task<StudentStudyExtension?> GetStudyExtensionAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return _dbContext.StudentStudyExtensions
            .FirstOrDefaultAsync(e => e.StudentId == studentId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<StudentHistoryRecord>> GetHistoryRecordsAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.StudentHistoryRecords
            .Where(h => h.StudentId == studentId)
            .OrderByDescending(h => h.EventDateUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<StudentProfilePicture?> GetProfilePictureAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return _dbContext.StudentProfilePictures
            .FirstOrDefaultAsync(p => p.StudentId == studentId, cancellationToken);
    }

    public async Task SaveProfilePictureAsync(StudentProfilePicture picture, CancellationToken cancellationToken = default)
    {
        var existing = await GetProfilePictureAsync(picture.StudentId, cancellationToken);
        if (existing is null)
        {
            await _dbContext.StudentProfilePictures.AddAsync(picture, cancellationToken);
        }
        else
        {
            _dbContext.StudentProfilePictures.Remove(existing);
            await _dbContext.StudentProfilePictures.AddAsync(picture, cancellationToken);
        }
    }

    public Task<StudentAlumniInfo?> GetAlumniInfoAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return _dbContext.StudentAlumniInfos
            .FirstOrDefaultAsync(a => a.StudentId == studentId, cancellationToken);
    }

    public async Task SaveAlumniInfoAsync(StudentAlumniInfo info, CancellationToken cancellationToken = default)
    {
        var existing = await GetAlumniInfoAsync(info.StudentId, cancellationToken);
        if (existing is null)
        {
            await _dbContext.StudentAlumniInfos.AddAsync(info, cancellationToken);
        }
        else
        {
            existing.Update(info.GraduationYear, info.CurrentCompany, info.CurrentPosition, info.LinkedInUrl, info.PersonalEmail, info.PhoneNumber, info.IsVisibleToOtherAlumni, DateTime.UtcNow);
        }
    }

    public Task<StudentAdvisorAssignment?> GetActiveAdvisorAssignmentAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return _dbContext.StudentAdvisorAssignments
            .Where(a => a.StudentId == studentId && a.IsActive)
            .OrderByDescending(a => a.AssignedDateUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<CourseOfferingLiveSession>> GetLiveSessionsAsync(IReadOnlyCollection<Guid> courseOfferingIds, CancellationToken cancellationToken = default)
    {
        if (courseOfferingIds.Count == 0)
        {
            return [];
        }

        return await _dbContext.CourseOfferingLiveSessions
            .Where(s => courseOfferingIds.Contains(s.CourseOfferingId) && s.IsActive)
            .OrderBy(s => s.StartTimeUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AcademicCalendarEvent>> GetAcademicCalendarEventsAsync(string? academicYear = null, CancellationToken cancellationToken = default)
    {
        IQueryable<AcademicCalendarEvent> query = _dbContext.AcademicCalendarEvents.Where(e => e.IsPublic);

        if (!string.IsNullOrWhiteSpace(academicYear))
        {
            query = query.Where(e => e.AcademicYear == academicYear);
        }

        return await query.OrderBy(e => e.StartDate).ToListAsync(cancellationToken);
    }
}
