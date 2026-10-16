using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;
using Academic.Application.Abstractions;

namespace Academic.Application.Features.StudentPortal;

public sealed record GetStudentWeeklyScheduleQuery : IRequest<ServiceResult<IReadOnlyCollection<StudentScheduleEntryDto>>>;

public sealed record StudentScheduleEntryDto(
    string DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string CourseCode,
    string CourseName,
    string SectionType, // Teori, Uygulama, Laboratuvar
    string FacultyName,
    string ClassroomName,
    string InstructorName);

public sealed class GetStudentWeeklyScheduleHandler(
    IStudentPortalRepository portalRepository,
    ICurrentUser currentUser) : IRequestHandler<GetStudentWeeklyScheduleQuery, ServiceResult<IReadOnlyCollection<StudentScheduleEntryDto>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<StudentScheduleEntryDto>>> Handle(
        GetStudentWeeklyScheduleQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<IReadOnlyCollection<StudentScheduleEntryDto>>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<IReadOnlyCollection<StudentScheduleEntryDto>>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        var schedule = new List<StudentScheduleEntryDto>
        {
            new(
                "Pazartesi",
                new TimeSpan(9, 30, 0),
                new TimeSpan(12, 20, 0),
                "CSE-301",
                "Elektronik Devreler ve Mantık Tasarımı",
                "Teori",
                "Bilgisayar ve Bilişim Fakültesi",
                "D101",
                "Prof. Dr. Ahmet Yılmaz"),
            new(
                "Salı",
                new TimeSpan(13, 30, 0),
                new TimeSpan(15, 20, 0),
                "CSE-301",
                "Elektronik Devreler ve Mantık Tasarımı",
                "Uygulama / Laboratuvar",
                "Elektrik-Elektronik Fakültesi",
                "LAB3",
                "Doç. Dr. Mehmet Demir"),
            new(
                "Çarşamba",
                new TimeSpan(10, 30, 0),
                new TimeSpan(13, 20, 0),
                "CSE-305",
                "İşletim Sistemleri",
                "Teori",
                "Bilgisayar ve Bilişim Fakültesi",
                "Amfi-1",
                "Prof. Dr. Canan Kaya"),
            new(
                "Perşembe",
                new TimeSpan(14, 30, 0),
                new TimeSpan(17, 20, 0),
                "CSE-307",
                "Veritabanı Yönetim Sistemleri",
                "Teori + Laboratuvar",
                "Bilgisayar ve Bilişim Fakültesi",
                "LAB2",
                "Dr. Öğr. Üyesi Selin Aksoy")
        };

        return ServiceResult<IReadOnlyCollection<StudentScheduleEntryDto>>.Ok(schedule);
    }
}
