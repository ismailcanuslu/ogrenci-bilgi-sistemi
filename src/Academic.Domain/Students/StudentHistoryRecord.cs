using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Students;

public sealed class StudentHistoryRecord : Entity<Guid>
{
    public const int TitleMaxLength = 150;
    public const int DescriptionMaxLength = 1000;
    public const int ActionByMaxLength = 100;

    public Guid StudentId { get; private set; }

    public string EventTitle { get; private set; } = null!;

    public string EventDescription { get; private set; } = null!;

    public DateTime EventDateUtc { get; private set; }

    public string? ActionBy { get; private set; }

    private StudentHistoryRecord()
    {
    }

    public StudentHistoryRecord(
        Guid id,
        Guid studentId,
        string eventTitle,
        string eventDescription,
        DateTime eventDateUtc,
        string? actionBy)
        : base(id)
    {
        if (studentId == Guid.Empty)
        {
            throw new DomainException("StudentHistoryRecord.StudentRequired");
        }

        if (string.IsNullOrWhiteSpace(eventTitle))
        {
            throw new DomainException("StudentHistoryRecord.TitleRequired");
        }

        StudentId = studentId;
        EventTitle = eventTitle.Trim();
        EventDescription = eventDescription.Trim();
        EventDateUtc = eventDateUtc;
        ActionBy = actionBy?.Trim();
    }
}
