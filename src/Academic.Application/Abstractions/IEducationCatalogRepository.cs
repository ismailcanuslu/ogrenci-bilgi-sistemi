using Academic.Domain.Courses;
using Academic.Domain.Curriculums;
using Academic.Domain.Faculties;
using Academic.Domain.Programs;

namespace Academic.Application.Abstractions;

public interface IEducationCatalogRepository
{
    Task<IReadOnlyCollection<CatalogFacultyNodeDto>> GetAcademicHierarchyTreeAsync(DegreeLevel? degreeLevel = null, CancellationToken cancellationToken = default);
    Task<ProgramProfileDto> GetProgramProfileAsync(Guid programId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<string>> GetProgramOutcomesAsync(Guid programId, CancellationToken cancellationToken = default);
    Task<CatalogTeachingPlanDto?> GetProgramTeachingPlanAsync(Guid programId, Guid? curriculumVersionId = null, CancellationToken cancellationToken = default);
    Task<CourseSyllabusDto?> GetCourseSyllabusAsync(string courseCode, CancellationToken cancellationToken = default);
    Task UpdateProgramProfileAsync(Guid programId, UpdateProgramProfileDto dto, CancellationToken cancellationToken = default);
    Task SaveCourseSyllabusAsync(CourseSyllabus syllabus, CancellationToken cancellationToken = default);
}

public record CatalogFacultyNodeDto(
    Guid FacultyId,
    string FacultyName,
    IReadOnlyCollection<CatalogDepartmentNodeDto> Departments);

public record CatalogDepartmentNodeDto(
    Guid DepartmentId,
    string DepartmentName,
    IReadOnlyCollection<CatalogProgramNodeDto> Programs);

public record CatalogProgramNodeDto(
    Guid ProgramId,
    string ProgramCode,
    string ProgramName,
    DegreeLevel DegreeLevel);

public record ProgramProfileDto(
    Guid ProgramId,
    string History,
    string AwardedDegree,
    string DegreeLevelDescription,
    string AdmissionRequirements,
    string RecognitionOfPriorLearning,
    string QualificationRequirements,
    string ProgramAims,
    string OccupationalProfiles,
    string AccessToFurtherStudies,
    string ExaminationRegulations,
    string GraduationRequirements,
    string ModeOfStudy,
    string DepartmentHeadAndContact,
    string Facilities,
    string ProgramEducationalObjectives,
    DateTime UpdatedAtUtc);

public record UpdateProgramProfileDto(
    string History,
    string AwardedDegree,
    string DegreeLevelDescription,
    string AdmissionRequirements,
    string RecognitionOfPriorLearning,
    string QualificationRequirements,
    string ProgramAims,
    string OccupationalProfiles,
    string AccessToFurtherStudies,
    string ExaminationRegulations,
    string GraduationRequirements,
    string ModeOfStudy,
    string DepartmentHeadAndContact,
    string Facilities,
    string ProgramEducationalObjectives,
    string ProgramOutcomesJson);

public record CatalogTeachingPlanDto(
    Guid CurriculumId,
    Guid VersionId,
    int VersionNumber,
    string CurriculumName,
    IReadOnlyCollection<CatalogSemesterGroupDto> Semesters);

public record CatalogSemesterGroupDto(
    int Semester,
    int TotalTheory,
    int TotalPractice,
    int TotalLab,
    decimal TotalLocalCredit,
    int TotalEcts,
    IReadOnlyCollection<CatalogCourseItemDto> Courses);

public record CatalogCourseItemDto(
    Guid Id,
    string CourseCode,
    string CourseName,
    string CourseType, // Zorunlu / Seçmeli
    string Language,
    int TheoryHours,
    int PracticeHours,
    int LabHours,
    decimal LocalCredit,
    int Ects,
    string? Prerequisites);

public record CourseSyllabusDto(
    string CourseCode,
    string CourseName,
    string Language,
    string Level,
    string Type,
    string PrerequisitesText,
    string Aim,
    string Content,
    string TextbooksAndResources,
    string PlannedLearningActivities,
    string RecommendedOtherTopics,
    string InstructorsText,
    IReadOnlyCollection<string> LearningOutcomes,
    int TheoryHours,
    int PracticeHours,
    int LabHours,
    decimal LocalCredit,
    int Ects,
    DateTime LastUpdatedAtUtc);
