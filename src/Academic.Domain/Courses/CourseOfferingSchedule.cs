using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Courses;

public sealed class CourseOfferingSchedule : Entity<Guid>
{
    public const int BuildingOrFacultyMaxLength = 100;
    public const int ClassroomMaxLength = 100;

    public Guid CourseOfferingId { get; private set; }

    public SessionType SessionType { get; private set; }

    public string BuildingOrFaculty { get; private set; } = null!;

    public string Classroom { get; private set; } = null!;

    public DayOfWeek DayOfWeek { get; private set; }

    public TimeSpan StartTime { get; private set; }

    public TimeSpan EndTime { get; private set; }

    public Guid? InstructorId { get; private set; }

    public bool IsOnline { get; private set; }

    private CourseOfferingSchedule()
    {
    }

    internal CourseOfferingSchedule(
        Guid id,
        Guid courseOfferingId,
        SessionType sessionType,
        string buildingOrFaculty,
        string classroom,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? instructorId,
        bool isOnline)
        : base(id)
    {
        if (startTime >= endTime)
        {
            throw new DomainException("CourseOfferingSchedule.InvalidTimeRange");
        }

        if (startTime < TimeSpan.Zero || endTime > TimeSpan.FromHours(24))
        {
            throw new DomainException("CourseOfferingSchedule.TimeOutOfBounds");
        }

        if (string.IsNullOrWhiteSpace(classroom))
        {
            throw new DomainException("CourseOfferingSchedule.ClassroomRequired");
        }

        var trimmedClassroom = classroom.Trim();
        if (trimmedClassroom.Length > ClassroomMaxLength)
        {
            throw new DomainException("CourseOfferingSchedule.ClassroomTooLong");
        }

        var trimmedBuilding = string.IsNullOrWhiteSpace(buildingOrFaculty) ? "Ana Kampüs" : buildingOrFaculty.Trim();
        if (trimmedBuilding.Length > BuildingOrFacultyMaxLength)
        {
            throw new DomainException("CourseOfferingSchedule.BuildingOrFacultyTooLong");
        }

        CourseOfferingId = courseOfferingId;
        SessionType = sessionType;
        BuildingOrFaculty = trimmedBuilding;
        Classroom = trimmedClassroom;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        InstructorId = instructorId;
        IsOnline = isOnline;
    }

    public bool OverlapsWith(DayOfWeek day, TimeSpan start, TimeSpan end)
    {
        return DayOfWeek == day && StartTime < end && EndTime > start;
    }
}
