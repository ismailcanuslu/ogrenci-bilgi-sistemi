using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;
using Academic.Application.Abstractions;

namespace Academic.Application.Features.StudentPortal;

public sealed record GetRegistrationApprovalReportQuery(string? AcademicTerm = null) : IRequest<ServiceResult<RegistrationApprovalReportDto>>;

public sealed record EnrolledCourseReportItemDto(
    string CourseCode,
    string CourseName,
    int Credits,
    int Ects,
    string InstructorName,
    string Status);

public sealed record RegistrationApprovalReportDto(
    Guid StudentId,
    string StudentNumber,
    string FullName,
    string AcademicYear,
    string Semester,
    bool IsAdvisorApproved,
    DateTime? AdvisorApprovedAtUtc,
    string AdvisorName,
    int TotalCredits,
    int TotalEcts,
    IReadOnlyCollection<EnrolledCourseReportItemDto> EnrolledCourses);

public sealed class GetRegistrationApprovalReportHandler(
    IStudentPortalRepository portalRepository,
    ICurrentUser currentUser) : IRequestHandler<GetRegistrationApprovalReportQuery, ServiceResult<RegistrationApprovalReportDto>>
{
    public async Task<ServiceResult<RegistrationApprovalReportDto>> Handle(
        GetRegistrationApprovalReportQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<RegistrationApprovalReportDto>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<RegistrationApprovalReportDto>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        var courses = new List<EnrolledCourseReportItemDto>
        {
            new("CSE-301", "Elektronik Devreler ve Mantık Tasarımı", 3, 6, "Prof. Dr. Ahmet Yılmaz", "Kayıtlı / Onaylı"),
            new("CSE-305", "İşletim Sistemleri", 3, 6, "Prof. Dr. Canan Kaya", "Kayıtlı / Onaylı"),
            new("CSE-307", "Veritabanı Yönetim Sistemleri", 3, 6, "Dr. Öğr. Üyesi Selin Aksoy", "Kayıtlı / Onaylı"),
            new("MATH-301", "Diferansiyel Denklemler", 3, 5, "Doç. Dr. Mustafa Koç", "Kayıtlı / Onaylı"),
            new("ENG-301", "Akademik Yazım ve Sunum Teknikleri", 2, 3, "Öğr. Gör. Burcu Şen", "Kayıtlı / Onaylı")
        };

        var report = new RegistrationApprovalReportDto(
            student.Id,
            student.StudentNumber,
            $"{student.FirstName} {student.LastName}",
            "2026-2027",
            "Güz",
            true,
            DateTime.UtcNow.AddDays(-15),
            "Prof. Dr. Akademik Danışman",
            14,
            26,
            courses);

        return ServiceResult<RegistrationApprovalReportDto>.Ok(report);
    }
}
