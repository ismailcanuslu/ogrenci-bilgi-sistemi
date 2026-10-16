using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Courses;

public enum LiveSessionType
{
    LiveLecture = 1, // Canlı Ders
    OnlineExam = 2   // Online Sınav
}

public sealed class CourseOfferingLiveSession : Entity<Guid>
{
    public const int TitleMaxLength = 200;
    public const int PlatformMaxLength = 50;
    public const int UrlMaxLength = 1000;
    public const int PasswordMaxLength = 50;

    public Guid CourseOfferingId { get; private set; }

    public string Title { get; private set; } = null!;

    public LiveSessionType SessionType { get; private set; }

    public string Platform { get; private set; } = null!; // Zoom, Teams, Meet vs.

    public string MeetingUrl { get; private set; } = null!;

    public string? AccessPassword { get; private set; }

    public DateTime StartTimeUtc { get; private set; }

    public DateTime EndTimeUtc { get; private set; }

    public bool IsActive { get; private set; }

    private CourseOfferingLiveSession()
    {
    }

    public CourseOfferingLiveSession(
        Guid id,
        Guid courseOfferingId,
        string title,
        LiveSessionType sessionType,
        string platform,
        string meetingUrl,
        string? accessPassword,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        bool isActive = true)
        : base(id)
    {
        if (courseOfferingId == Guid.Empty)
        {
            throw new DomainException("CourseOfferingLiveSession.CourseOfferingRequired");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("CourseOfferingLiveSession.TitleRequired");
        }

        if (string.IsNullOrWhiteSpace(meetingUrl))
        {
            throw new DomainException("CourseOfferingLiveSession.MeetingUrlRequired");
        }

        if (startTimeUtc >= endTimeUtc)
        {
            throw new DomainException("CourseOfferingLiveSession.InvalidTimeRange");
        }

        CourseOfferingId = courseOfferingId;
        Title = title.Trim();
        SessionType = sessionType;
        Platform = string.IsNullOrWhiteSpace(platform) ? "Zoom" : platform.Trim();
        MeetingUrl = meetingUrl.Trim();
        AccessPassword = accessPassword?.Trim();
        StartTimeUtc = startTimeUtc;
        EndTimeUtc = endTimeUtc;
        IsActive = isActive;
    }
}
