using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Curriculums;

public sealed class CurriculumVersion : Entity<Guid>
{
    private readonly List<CurriculumCourse> _courses = [];

    public Guid CurriculumId { get; private set; }

    public int VersionNumber { get; private set; }

    public CurriculumVersionStatus Status { get; private set; }

    public string? Notes { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? PublishedAtUtc { get; private set; }

    public IReadOnlyCollection<CurriculumCourse> Courses => _courses.AsReadOnly();

    public int TotalEcts => _courses.Sum(c => c.Ects);

    private CurriculumVersion()
    {
    }

    internal CurriculumVersion(
        Guid id,
        Guid curriculumId,
        int versionNumber,
        string? notes,
        DateTime createdAtUtc)
        : base(id)
    {
        CurriculumId = curriculumId;
        VersionNumber = versionNumber;
        Status = CurriculumVersionStatus.Draft;
        Notes = notes?.Trim();
        CreatedAtUtc = createdAtUtc;
    }

    public CurriculumCourse AddCourse(
        string courseCode,
        string courseName,
        int ects,
        int credit,
        int semester,
        bool isElective,
        CourseCategory category,
        string? prerequisites)
    {
        EnsureDraft();

        var normalizedCode = courseCode.Trim().ToUpperInvariant();

        if (_courses.Any(c => c.CourseCode == normalizedCode && c.Semester == semester))
        {
            throw new DomainException("CurriculumVersion.CourseAlreadyInSemester");
        }

        var course = new CurriculumCourse(
            Guid.NewGuid(),
            Id,
            courseCode,
            courseName,
            ects,
            credit,
            semester,
            isElective,
            category,
            prerequisites);

        _courses.Add(course);
        return course;
    }

    public void RemoveCourse(Guid courseId)
    {
        EnsureDraft();

        var course = _courses.FirstOrDefault(c => c.Id == courseId);
        if (course is null)
        {
            throw new DomainException("CurriculumVersion.CourseNotFound");
        }

        _courses.Remove(course);
    }

    public void Publish(DateTime publishedAtUtc)
    {
        EnsureDraft();

        if (_courses.Count == 0)
        {
            throw new DomainException("CurriculumVersion.CannotPublishEmptyCurriculum");
        }

        Status = CurriculumVersionStatus.Published;
        PublishedAtUtc = publishedAtUtc;
    }

    public void Archive()
    {
        if (Status != CurriculumVersionStatus.Published)
        {
            throw new DomainException("CurriculumVersion.CannotArchiveNonPublished");
        }

        Status = CurriculumVersionStatus.Archived;
    }

    private void EnsureDraft()
    {
        if (Status != CurriculumVersionStatus.Draft)
        {
            throw new DomainException("CurriculumVersion.ImmutableWhenNotDraft");
        }
    }
}
