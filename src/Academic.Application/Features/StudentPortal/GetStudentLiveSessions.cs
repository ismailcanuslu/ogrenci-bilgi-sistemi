using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;
using Academic.Application.Abstractions;
using Academic.Domain.Courses;

namespace Academic.Application.Features.StudentPortal;

public sealed record GetStudentLiveSessionsQuery(LiveSessionType? SessionType = null) : IRequest<ServiceResult<IReadOnlyCollection<StudentLiveSessionDto>>>;

public sealed record StudentLiveSessionDto(
    Guid Id,
    Guid CourseOfferingId,
    string CourseCode,
    string CourseName,
    string Title,
    string SessionType,
    string Platform,
    string MeetingUrl,
    string? AccessPassword,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc,
    bool IsActive);

public sealed class GetStudentLiveSessionsHandler(
    IStudentPortalRepository portalRepository,
    ICurrentUser currentUser) : IRequestHandler<GetStudentLiveSessionsQuery, ServiceResult<IReadOnlyCollection<StudentLiveSessionDto>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<StudentLiveSessionDto>>> Handle(
        GetStudentLiveSessionsQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<IReadOnlyCollection<StudentLiveSessionDto>>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<IReadOnlyCollection<StudentLiveSessionDto>>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        // Örnek aktif ders canlı oturumu
        var sessions = new List<StudentLiveSessionDto>();

        if (!request.SessionType.HasValue || request.SessionType == LiveSessionType.LiveLecture)
        {
            sessions.Add(new StudentLiveSessionDto(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "CSE-301",
                "Elektronik Devreler ve Mantık Tasarımı",
                "Hafta 4: Mantık Kapıları ve Karnaugh Haritaları",
                "LiveLecture",
                "Zoom",
                "https://zoom.us/j/9876543210?pwd=abc",
                "123456",
                DateTime.UtcNow.AddHours(1),
                DateTime.UtcNow.AddHours(3),
                true));
        }

        if (!request.SessionType.HasValue || request.SessionType == LiveSessionType.OnlineExam)
        {
            sessions.Add(new StudentLiveSessionDto(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "CSE-305",
                "İşletim Sistemleri",
                "Online Ara Sınav (Vize)",
                "OnlineExam",
                "InternalExamPortal",
                "https://sinav.university.edu.tr/exam/cse305-midterm",
                null,
                DateTime.UtcNow.AddDays(2),
                DateTime.UtcNow.AddDays(2).AddHours(2),
                true));
        }

        return ServiceResult<IReadOnlyCollection<StudentLiveSessionDto>>.Ok(sessions);
    }
}
