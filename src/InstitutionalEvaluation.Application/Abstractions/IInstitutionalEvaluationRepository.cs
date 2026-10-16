namespace InstitutionalEvaluation.Application.Abstractions;

public interface IInstitutionalEvaluationRepository
{
    Task<InstitutionalDashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<StudentStatisticsDto> GetStudentStatisticsAsync(CancellationToken cancellationToken = default);
    Task<AlumniStatisticsDto> GetAlumniStatisticsAsync(CancellationToken cancellationToken = default);
    Task<StaffStatisticsDto> GetStaffStatisticsAsync(CancellationToken cancellationToken = default);
    Task<PublicationStatisticsDto> GetPublicationStatisticsAsync(CancellationToken cancellationToken = default);
    Task<UnitStatisticsDto> GetUnitStatisticsAsync(CancellationToken cancellationToken = default);
    Task<KeyPerformanceIndicatorsDto> GetIndicatorsAsync(CancellationToken cancellationToken = default);
    Task<BapStatisticsDto> GetBapStatisticsAsync(CancellationToken cancellationToken = default);
    Task<DistanceEducationStatisticsDto> GetDistanceEducationStatisticsAsync(CancellationToken cancellationToken = default);
}

public record MetricBreakdownItemDto(string Label, decimal Value, decimal Percentage = 0);

public record InstitutionalDashboardSummaryDto(
    int TotalStudents,
    int TotalAlumni,
    int TotalAcademicStaff,
    int TotalAdministrativeStaff,
    int TotalUnits,
    int TotalPublications,
    int TotalBapProjects,
    decimal TotalPhysicalAreaM2);

public record StudentStatisticsDto(
    int TotalStudents,
    IReadOnlyCollection<MetricBreakdownItemDto> ByProgram,
    IReadOnlyCollection<MetricBreakdownItemDto> ByUnit,
    int AssociateDegreeCount, // Ön Lisans
    int UndergraduateCount,   // Lisans
    int GraduateCount,        // Lisansüstü (YL + Doktora)
    int InternationalCount,   // Uluslararası
    int PrepClassCount,       // Hazırlık
    IReadOnlyCollection<MetricBreakdownItemDto> ByRegion, // İl / Bölge bazında
    IReadOnlyCollection<MetricBreakdownItemDto> ByYearLevel); // Sınıf bazında (1., 2., 3., 4. Sınıf)

public record AlumniStatisticsDto(
    int TotalAlumni,
    IReadOnlyCollection<MetricBreakdownItemDto> ByGraduationYear,
    IReadOnlyCollection<MetricBreakdownItemDto> ByUnit);

public record StaffStatisticsDto(
    int TotalAcademicStaff,
    int TotalAdministrativeStaff,
    IReadOnlyCollection<MetricBreakdownItemDto> AcademicStaffByDutyPlace, // Görev yerine göre
    IReadOnlyCollection<MetricBreakdownItemDto> AcademicStaffByCadrePlace, // Kadro yerine göre
    IReadOnlyCollection<MetricBreakdownItemDto> AdministrativeStaffByDutyPlace,
    IReadOnlyCollection<MetricBreakdownItemDto> AdministrativeStaffByCadrePlace);

public record PublicationStatisticsDto(
    int TotalPublications,
    int SciExpandedCount,
    int ScopusCount,
    int NationalRefereedCount,
    int BooksAndChaptersCount,
    IReadOnlyCollection<MetricBreakdownItemDto> ByYear);

public record UnitStatisticsDto(
    int TotalFaculties,
    int TotalInstitutes,
    int TotalVocationalSchools,
    int TotalResearchCenters,
    int TotalAcademicPrograms,
    IReadOnlyCollection<MetricBreakdownItemDto> ProgramsByDegreeLevel);

public record KeyPerformanceIndicatorsDto(
    decimal StudentPerFacultyRatio, // Öğrenci / Öğretim Üyesi Oranı
    decimal InternationalStudentPercentage,
    decimal TotalCampusAreaM2,
    decimal IndoorAreaM2,
    decimal GreenAreaPerStudentM2,
    decimal LaboratoryAreaPerStudentM2,
    decimal BolognaCompliancePercentage); // Bologna Göstergeleri

public record BapStatisticsDto(
    int TotalProjects,
    int OngoingProjects,
    int CompletedProjects,
    decimal TotalBudgetGranted,
    IReadOnlyCollection<MetricBreakdownItemDto> ByProjectType);

public record DistanceEducationStatisticsDto(
    int ActiveOnlineCoursesCount,
    int TotalSynchronousSessionsHeld,
    int TotalAsynchronousVideosPublished,
    int OnlineExamsConducted,
    decimal AverageAttendanceRate);
