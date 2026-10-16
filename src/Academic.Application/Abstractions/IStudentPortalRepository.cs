using Academic.Domain.AcademicCalendars;
using Academic.Domain.Courses;
using Academic.Domain.Students;

namespace Academic.Application.Abstractions;

public interface IStudentPortalRepository
{
    Task<Student?> GetStudentByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Student?> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken = default);

    // Diğer Menüsü Bilgileri
    Task<IReadOnlyCollection<StudentTuition>> GetTuitionsAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<StudentMilitaryStatus?> GetMilitaryStatusAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<StudentDisciplinaryRecord>> GetDisciplinaryRecordsAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<StudentBankDetails?> GetBankDetailsAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task SaveBankDetailsAsync(StudentBankDetails details, CancellationToken cancellationToken = default);
    Task<StudentStudyExtension?> GetStudyExtensionAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<StudentHistoryRecord>> GetHistoryRecordsAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<StudentProfilePicture?> GetProfilePictureAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task SaveProfilePictureAsync(StudentProfilePicture picture, CancellationToken cancellationToken = default);
    Task<StudentAlumniInfo?> GetAlumniInfoAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task SaveAlumniInfoAsync(StudentAlumniInfo info, CancellationToken cancellationToken = default);
    Task<StudentAdvisorAssignment?> GetActiveAdvisorAssignmentAsync(Guid studentId, CancellationToken cancellationToken = default);

    // Canlı Dersler, Haftalık Program ve Takvim
    Task<IReadOnlyCollection<CourseOfferingLiveSession>> GetLiveSessionsAsync(IReadOnlyCollection<Guid> courseOfferingIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AcademicCalendarEvent>> GetAcademicCalendarEventsAsync(string? academicYear = null, CancellationToken cancellationToken = default);
}
