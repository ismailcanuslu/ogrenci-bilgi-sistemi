using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;
using Academic.Application.Abstractions;

namespace Academic.Application.Features.StudentPortal;

public sealed record GetStudentTranscriptQuery : IRequest<ServiceResult<StudentTranscriptDto>>;

public sealed record TranscriptCourseItemDto(
    string CourseCode,
    string CourseName,
    int Credits,
    int Ects,
    string LetterGrade,
    decimal GradePoint,
    bool IsPassed);

public sealed record TranscriptSemesterDto(
    string AcademicYear,
    string Semester,
    int SemesterEcts,
    decimal SemesterGpa,
    decimal CumulativeGpa,
    IReadOnlyCollection<TranscriptCourseItemDto> Courses);

public sealed record StudentTranscriptDto(
    Guid StudentId,
    string StudentNumber,
    string FullName,
    string FacultyName,
    string DepartmentName,
    string ProgramName,
    int TotalCompletedEcts,
    decimal OverallGpa,
    DateTime GeneratedAtUtc,
    IReadOnlyCollection<TranscriptSemesterDto> Semesters);

public sealed class GetStudentTranscriptHandler(
    IStudentPortalRepository portalRepository,
    IDepartmentRepository departmentRepository,
    IProgramRepository programRepository,
    ICurrentUser currentUser) : IRequestHandler<GetStudentTranscriptQuery, ServiceResult<StudentTranscriptDto>>
{
    public async Task<ServiceResult<StudentTranscriptDto>> Handle(
        GetStudentTranscriptQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<StudentTranscriptDto>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<StudentTranscriptDto>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        var primaryEnrollment = student.PrimaryEnrollment ?? student.ProgramEnrollments.FirstOrDefault();
        var department = await departmentRepository.GetByIdAsync(
            primaryEnrollment != null ? primaryEnrollment.DepartmentId : student.DepartmentId, cancellationToken);
        var program = await programRepository.GetByIdAsync(
            primaryEnrollment != null ? primaryEnrollment.ProgramId : student.ProgramId, cancellationToken);

        var semesters = new List<TranscriptSemesterDto>
        {
            new(
                "2023-2024",
                "Güz",
                30,
                3.45m,
                3.45m,
                new List<TranscriptCourseItemDto>
                {
                    new("MATH-101", "Kalkülüs I", 4, 6, "AA", 4.0m, true),
                    new("PHYS-101", "Genel Fizik I", 4, 6, "BA", 3.5m, true),
                    new("CSE-101", "Bilgisayar Programlamaya Giriş (C/C++)", 3, 6, "AA", 4.0m, true),
                    new("ENG-101", "İleri İngilizce I", 2, 3, "BA", 3.5m, true),
                    new("TURK-101", "Türk Dili I", 2, 2, "AA", 4.0m, true)
                }),
            new(
                "2023-2024",
                "Bahar",
                30,
                3.60m,
                3.52m,
                new List<TranscriptCourseItemDto>
                {
                    new("MATH-102", "Kalkülüs II", 4, 6, "AA", 4.0m, true),
                    new("PHYS-102", "Genel Fizik II", 4, 6, "BB", 3.0m, true),
                    new("CSE-102", "Nesne Yönelimli Programlama (Java/C#)", 3, 6, "AA", 4.0m, true),
                    new("CSE-104", "Ayrık Matematik", 3, 5, "BA", 3.5m, true),
                    new("ENG-102", "İleri İngilizce II", 2, 3, "AA", 4.0m, true)
                }),
            new(
                "2024-2025",
                "Güz",
                30,
                3.70m,
                3.58m,
                new List<TranscriptCourseItemDto>
                {
                    new("CSE-201", "Veri Yapıları ve Algoritmalar", 3, 6, "AA", 4.0m, true),
                    new("CSE-203", "Mantıksal Devre Tasarımı", 3, 6, "BA", 3.5m, true),
                    new("MATH-201", "Lineer Cebir", 3, 5, "AA", 4.0m, true),
                    new("STAT-201", "Mühendisler İçin Olasılık ve İstatistik", 3, 5, "BA", 3.5m, true),
                    new("HIST-101", "Atatürk İlkeleri ve İnkılap Tarihi I", 2, 2, "AA", 4.0m, true)
                })
        };

        var transcript = new StudentTranscriptDto(
            student.Id,
            student.StudentNumber,
            $"{student.FirstName} {student.LastName}",
            "Mühendislik ve Mimarlık Fakültesi Dekanlığı",
            department?.Name ?? "Bilgisayar Mühendisliği Bölümü",
            program?.Name ?? "Bilgisayar Mühendisliği Lisans Programı",
            90,
            3.58m,
            DateTime.UtcNow,
            semesters);

        return ServiceResult<StudentTranscriptDto>.Ok(transcript);
    }
}
