using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Courses;

public sealed class CourseOffering : AggregateRoot<Guid>
{
    private readonly List<CourseOfferingInstructor> _instructors = [];
    private readonly List<CourseOfferingSchedule> _schedules = [];

    public Guid CourseId { get; private set; }

    public Guid AcademicTermId { get; private set; }

    public int Section { get; private set; }

    public int Capacity { get; private set; }

    public int EnrolledCount { get; private set; }

    public bool IsRegistrationOpen { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<CourseOfferingInstructor> Instructors => _instructors.AsReadOnly();

    public IReadOnlyCollection<CourseOfferingSchedule> Schedules => _schedules.AsReadOnly();

    public Guid? PrimaryInstructorId => _instructors.FirstOrDefault(i => i.IsPrimary)?.InstructorId;

    private CourseOffering()
    {
    }

    private CourseOffering(
        Guid id,
        Guid courseId,
        Guid academicTermId,
        int section,
        int capacity,
        DateTime createdAtUtc)
        : base(id)
    {
        CourseId = courseId;
        AcademicTermId = academicTermId;
        Section = ValidateSection(section);
        Capacity = ValidateCapacity(capacity);
        EnrolledCount = 0;
        IsRegistrationOpen = true;
        CreatedAtUtc = createdAtUtc;
    }

    public static CourseOffering Create(
        Guid courseId,
        Guid academicTermId,
        int section,
        int capacity,
        Guid? primaryInstructorId,
        DateTime createdAtUtc)
    {
        if (courseId == Guid.Empty)
        {
            throw new DomainException("CourseOffering.CourseRequired");
        }

        if (academicTermId == Guid.Empty)
        {
            throw new DomainException("CourseOffering.AcademicTermRequired");
        }

        var offering = new CourseOffering(
            Guid.NewGuid(),
            courseId,
            academicTermId,
            section,
            capacity,
            createdAtUtc);

        if (primaryInstructorId.HasValue && primaryInstructorId.Value != Guid.Empty)
        {
            offering.AssignInstructor(primaryInstructorId.Value, true, "Baş Eğitmen", createdAtUtc);
        }

        return offering;
    }

    public void AssignInstructor(Guid instructorId, DateTime assignedAtUtc)
        => AssignInstructor(instructorId, _instructors.Count == 0, null, assignedAtUtc);

    public void AssignInstructor(
        Guid instructorId,
        bool isPrimary,
        string? roleOrTitle,
        DateTime assignedAtUtc)
    {
        if (instructorId == Guid.Empty)
        {
            throw new DomainException("CourseOffering.InstructorRequired");
        }

        if (isPrimary)
        {
            foreach (var existing in _instructors.Where(i => i.IsPrimary))
            {
                existing.SetPrimary(false);
            }
        }

        var existingInstructor = _instructors.FirstOrDefault(i => i.InstructorId == instructorId);
        if (existingInstructor is not null)
        {
            existingInstructor.SetPrimary(isPrimary);
            existingInstructor.UpdateRole(roleOrTitle);
        }
        else
        {
            _instructors.Add(new CourseOfferingInstructor(
                Guid.NewGuid(),
                Id,
                instructorId,
                isPrimary,
                roleOrTitle,
                assignedAtUtc));
        }

        UpdatedAtUtc = assignedAtUtc;
    }

    public void RemoveInstructor(Guid instructorId, DateTime updatedAtUtc)
    {
        var instructor = _instructors.FirstOrDefault(i => i.InstructorId == instructorId);
        if (instructor is null)
        {
            throw new DomainException("CourseOffering.InstructorNotFound");
        }

        _instructors.Remove(instructor);

        // Eğer silinen birincil eğitmense ve geriye başka eğitmen kalmışsa ilkini birincil yap
        if (instructor.IsPrimary && _instructors.Count > 0)
        {
            _instructors[0].SetPrimary(true);
        }

        UpdatedAtUtc = updatedAtUtc;
    }

    public bool IsInstructorAssigned(Guid instructorId)
    {
        return _instructors.Any(i => i.InstructorId == instructorId) ||
               _schedules.Any(s => s.InstructorId == instructorId);
    }

    public void UpdateCapacity(int newCapacity, DateTime updatedAtUtc)
    {
        ValidateCapacity(newCapacity);

        if (newCapacity < EnrolledCount)
        {
            throw new DomainException("CourseOffering.CapacityCannotBeLessThanEnrolled");
        }

        Capacity = newCapacity;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void SetRegistrationStatus(bool isOpen, DateTime updatedAtUtc)
    {
        IsRegistrationOpen = isOpen;
        UpdatedAtUtc = updatedAtUtc;
    }

    public CourseOfferingSchedule AddSchedule(
        SessionType sessionType,
        string buildingOrFaculty,
        string classroom,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? instructorId,
        bool isOnline)
    {
        if (_schedules.Any(s => s.OverlapsWith(dayOfWeek, startTime, endTime)))
        {
            throw new DomainException("CourseOffering.ScheduleConflictInSameSection");
        }

        var schedule = new CourseOfferingSchedule(
            Guid.NewGuid(),
            Id,
            sessionType,
            buildingOrFaculty,
            classroom,
            dayOfWeek,
            startTime,
            endTime,
            instructorId,
            isOnline);

        _schedules.Add(schedule);
        return schedule;
    }

    public void RemoveSchedule(Guid scheduleId)
    {
        var schedule = _schedules.FirstOrDefault(s => s.Id == scheduleId);
        if (schedule is null)
        {
            throw new DomainException("CourseOffering.ScheduleNotFound");
        }

        _schedules.Remove(schedule);
    }

    public void IncrementEnrolled()
    {
        if (EnrolledCount >= Capacity)
        {
            throw new DomainException("CourseOffering.CapacityExceeded");
        }

        EnrolledCount++;
    }

    public void DecrementEnrolled()
    {
        if (EnrolledCount > 0)
        {
            EnrolledCount--;
        }
    }

    private static int ValidateSection(int section)
    {
        if (section < 1)
        {
            throw new DomainException("CourseOffering.InvalidSection");
        }

        return section;
    }

    private static int ValidateCapacity(int capacity)
    {
        if (capacity < 1)
        {
            throw new DomainException("CourseOffering.InvalidCapacity");
        }

        return capacity;
    }
}
