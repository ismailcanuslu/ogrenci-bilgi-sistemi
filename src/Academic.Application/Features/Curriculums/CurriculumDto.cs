using Academic.Domain.Curriculums;

namespace Academic.Application.Features.Curriculums;

public sealed record CurriculumCourseDto(
    Guid Id,
    Guid CurriculumVersionId,
    string CourseCode,
    string CourseName,
    int Ects,
    int Credit,
    int Semester,
    bool IsElective,
    CourseCategory Category,
    string? Prerequisites)
{
    public static CurriculumCourseDto From(CurriculumCourse course)
        => new(
            course.Id,
            course.CurriculumVersionId,
            course.CourseCode,
            course.CourseName,
            course.Ects,
            course.Credit,
            course.Semester,
            course.IsElective,
            course.Category,
            course.Prerequisites);
}

public sealed record CurriculumVersionDto(
    Guid Id,
    Guid CurriculumId,
    int VersionNumber,
    CurriculumVersionStatus Status,
    string? Notes,
    DateTime CreatedAtUtc,
    DateTime? PublishedAtUtc,
    int TotalEcts,
    IReadOnlyCollection<CurriculumCourseDto> Courses)
{
    public static CurriculumVersionDto From(CurriculumVersion version)
        => new(
            version.Id,
            version.CurriculumId,
            version.VersionNumber,
            version.Status,
            version.Notes,
            version.CreatedAtUtc,
            version.PublishedAtUtc,
            version.TotalEcts,
            version.Courses.Select(CurriculumCourseDto.From).ToArray());
}

public sealed record CurriculumDto(
    Guid Id,
    Guid ProgramId,
    Guid AcademicYearId,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    IReadOnlyCollection<CurriculumVersionDto> Versions)
{
    public static CurriculumDto From(Curriculum curriculum)
        => new(
            curriculum.Id,
            curriculum.ProgramId,
            curriculum.AcademicYearId,
            curriculum.Code,
            curriculum.Name,
            curriculum.IsActive,
            curriculum.CreatedAtUtc,
            curriculum.UpdatedAtUtc,
            curriculum.Versions.Select(CurriculumVersionDto.From).ToArray());
}
