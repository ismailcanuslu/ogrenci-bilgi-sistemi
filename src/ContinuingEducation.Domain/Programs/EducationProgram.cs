using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace ContinuingEducation.Domain.Programs;

public sealed class EducationProgram : AggregateRoot<Guid>
{
    public const int TitleMaxLength = 200;
    public const int CodeMaxLength = 30;
    public const int DescriptionMaxLength = 2000;

    public string Title { get; private set; } = null!;

    public string Code { get; private set; } = null!;

    public string? Description { get; private set; }

    public Guid InstructorUserId { get; private set; }

    public int Capacity { get; private set; }

    public DateTime StartDateUtc { get; private set; }

    public DateTime EndDateUtc { get; private set; }

    public int TotalHours { get; private set; }

    public decimal Price { get; private set; }

    public bool IsFree { get; private set; }

    public int MinAttendancePercentage { get; private set; }

    public decimal MinPassingGrade { get; private set; }

    public CourseProgramStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private EducationProgram()
    {
    }

    public EducationProgram(
        Guid id,
        string title,
        string code,
        string? description,
        Guid instructorUserId,
        int capacity,
        DateTime startDateUtc,
        DateTime endDateUtc,
        int totalHours,
        decimal price,
        bool isFree,
        int minAttendancePercentage,
        decimal minPassingGrade,
        DateTime createdAtUtc)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("EducationProgram.TitleRequired");
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("EducationProgram.CodeRequired");
        }

        if (instructorUserId == Guid.Empty)
        {
            throw new DomainException("EducationProgram.InstructorRequired");
        }

        if (capacity < 1)
        {
            throw new DomainException("EducationProgram.InvalidCapacity");
        }

        if (startDateUtc >= endDateUtc)
        {
            throw new DomainException("EducationProgram.InvalidDateRange");
        }

        if (totalHours < 1)
        {
            throw new DomainException("EducationProgram.InvalidTotalHours");
        }

        if (minAttendancePercentage < 0 || minAttendancePercentage > 100)
        {
            throw new DomainException("EducationProgram.InvalidAttendancePercentage");
        }

        Title = title.Trim();
        Code = code.Trim().ToUpperInvariant();
        Description = description?.Trim();
        InstructorUserId = instructorUserId;
        Capacity = capacity;
        StartDateUtc = startDateUtc;
        EndDateUtc = endDateUtc;
        TotalHours = totalHours;
        Price = price < 0 ? 0 : price;
        IsFree = isFree;
        MinAttendancePercentage = minAttendancePercentage;
        MinPassingGrade = minPassingGrade;
        Status = CourseProgramStatus.Active;
        CreatedAtUtc = createdAtUtc;
    }

    public static EducationProgram Create(
        string title,
        string code,
        string? description,
        Guid instructorUserId,
        int capacity,
        DateTime startDateUtc,
        DateTime endDateUtc,
        int totalHours,
        decimal price,
        bool isFree,
        int minAttendancePercentage,
        decimal minPassingGrade,
        DateTime createdAtUtc)
    {
        return new EducationProgram(
            Guid.NewGuid(),
            title,
            code,
            description,
            instructorUserId,
            capacity,
            startDateUtc,
            endDateUtc,
            totalHours,
            price,
            isFree,
            minAttendancePercentage,
            minPassingGrade,
            createdAtUtc);
    }
}
