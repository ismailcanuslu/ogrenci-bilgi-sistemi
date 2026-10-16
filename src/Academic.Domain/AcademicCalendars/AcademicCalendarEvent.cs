using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.AcademicCalendars;

public sealed class AcademicCalendarEvent : Entity<Guid>
{
    public const int TitleMaxLength = 200;
    public const int AcademicYearMaxLength = 20;

    public string Title { get; private set; } = null!;

    public string? Description { get; private set; }

    public DateOnly StartDate { get; private set; }

    public DateOnly EndDate { get; private set; }

    public string AcademicYear { get; private set; } = null!; // "2026-2027"

    public string Semester { get; private set; } = null!; // "Güz", "Bahar", "Yaz"

    public bool IsPublic { get; private set; }

    private AcademicCalendarEvent()
    {
    }

    public AcademicCalendarEvent(
        Guid id,
        string title,
        string? description,
        DateOnly startDate,
        DateOnly endDate,
        string academicYear,
        string semester,
        bool isPublic = true)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("AcademicCalendarEvent.TitleRequired");
        }

        if (startDate > endDate)
        {
            throw new DomainException("AcademicCalendarEvent.InvalidDateRange");
        }

        Title = title.Trim();
        Description = description?.Trim();
        StartDate = startDate;
        EndDate = endDate;
        AcademicYear = academicYear.Trim();
        Semester = semester.Trim();
        IsPublic = isPublic;
    }
}
