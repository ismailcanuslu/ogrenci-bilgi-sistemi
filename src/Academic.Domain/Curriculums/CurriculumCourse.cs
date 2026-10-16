using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Curriculums;

public sealed class CurriculumCourse : Entity<Guid>
{
    public const int CodeMaxLength = 20;
    public const int NameMaxLength = 200;

    public Guid CurriculumVersionId { get; private set; }

    public string CourseCode { get; private set; } = null!;

    public string CourseName { get; private set; } = null!;

    public int Ects { get; private set; }

    public int Credit { get; private set; }

    public int Semester { get; private set; }

    public bool IsElective { get; private set; }

    public CourseCategory Category { get; private set; }

    public string? Prerequisites { get; private set; }

    private CurriculumCourse()
    {
    }

    internal CurriculumCourse(
        Guid id,
        Guid curriculumVersionId,
        string courseCode,
        string courseName,
        int ects,
        int credit,
        int semester,
        bool isElective,
        CourseCategory category,
        string? prerequisites)
        : base(id)
    {
        CurriculumVersionId = curriculumVersionId;
        CourseCode = NormalizeCode(courseCode);
        CourseName = NormalizeName(courseName);

        if (ects <= 0 || ects > 60)
        {
            throw new DomainException("CurriculumCourse.InvalidEcts");
        }

        if (credit < 0 || credit > 30)
        {
            throw new DomainException("CurriculumCourse.InvalidCredit");
        }

        if (semester <= 0 || semester > 12)
        {
            throw new DomainException("CurriculumCourse.InvalidSemester");
        }

        if (!Enum.IsDefined(category))
        {
            throw new DomainException("CurriculumCourse.InvalidCategory");
        }

        Ects = ects;
        Credit = credit;
        Semester = semester;
        IsElective = isElective;
        Category = category;
        Prerequisites = prerequisites?.Trim();
    }

    private static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("CurriculumCourse.CodeRequired");
        }

        var trimmed = code.Trim().ToUpperInvariant();
        if (trimmed.Length > CodeMaxLength)
        {
            throw new DomainException("CurriculumCourse.CodeTooLong");
        }

        return trimmed;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("CurriculumCourse.NameRequired");
        }

        var trimmed = name.Trim();
        if (trimmed.Length > NameMaxLength)
        {
            throw new DomainException("CurriculumCourse.NameTooLong");
        }

        return trimmed;
    }
}
