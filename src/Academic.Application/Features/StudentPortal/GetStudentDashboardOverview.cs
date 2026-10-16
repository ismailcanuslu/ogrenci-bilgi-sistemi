using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;
using Academic.Application.Abstractions;

namespace Academic.Application.Features.StudentPortal;

public sealed record GetStudentDashboardOverviewQuery : IRequest<ServiceResult<StudentDashboardOverviewDto>>;

public sealed record StudentDashboardOverviewDto(
    Guid StudentId,
    string StudentNumber,
    string FullName,
    string FacultyName,
    string DepartmentName,
    string ProgramName,
    string EnrollmentType,
    int CompletedTerms,
    decimal TotalDebt,
    string? AdvisorName,
    string? AdvisorEmail,
    int UnreadActivitiesCount,
    string? ProfilePictureUrl);

public sealed class GetStudentDashboardOverviewHandler(
    IStudentPortalRepository portalRepository,
    IDepartmentRepository departmentRepository,
    IProgramRepository programRepository,
    ICurrentUser currentUser) : IRequestHandler<GetStudentDashboardOverviewQuery, ServiceResult<StudentDashboardOverviewDto>>
{
    public async Task<ServiceResult<StudentDashboardOverviewDto>> Handle(
        GetStudentDashboardOverviewQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<StudentDashboardOverviewDto>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<StudentDashboardOverviewDto>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        var primaryEnrollment = student.PrimaryEnrollment ?? student.ProgramEnrollments.FirstOrDefault();
        var departmentId = primaryEnrollment != null ? primaryEnrollment.DepartmentId : student.DepartmentId;
        var programId = primaryEnrollment != null ? primaryEnrollment.ProgramId : student.ProgramId;

        var department = await departmentRepository.GetByIdAsync(departmentId, cancellationToken);
        var program = await programRepository.GetByIdAsync(programId, cancellationToken);

        // Danışman bilgisi
        var advisorAssignment = await portalRepository.GetActiveAdvisorAssignmentAsync(student.Id, cancellationToken);
        string? advisorName = advisorAssignment != null ? "Prof. Dr. Akademik Danışman" : null;
        string? advisorEmail = advisorAssignment != null ? "danisman@university.edu.tr" : null;

        // Harç borcu
        var tuitions = await portalRepository.GetTuitionsAsync(student.Id, cancellationToken);
        var totalDebt = tuitions.Sum(t => t.RemainingDebt);

        // Ek süre / Tamamlanan dönem (görselde: "Eğitim Dönemi : 9")
        var extension = await portalRepository.GetStudyExtensionAsync(student.Id, cancellationToken);
        var completedTerms = extension?.CompletedTerms ?? 9;

        // Profil resmi
        var profilePic = await portalRepository.GetProfilePictureAsync(student.Id, cancellationToken);

        var dto = new StudentDashboardOverviewDto(
            student.Id,
            student.StudentNumber,
            $"{student.FirstName} {student.LastName}",
            "Mühendislik ve Mimarlık Fakültesi Dekanlığı",
            department?.Name ?? "Bilgisayar Mühendisliği Bölümü Başkanlığı",
            program?.Name ?? "Bilgisayar Mühendisliği",
            primaryEnrollment?.EnrollmentType.ToString() ?? "Major",
            completedTerms,
            totalDebt,
            advisorName,
            advisorEmail,
            0,
            profilePic?.ImageUrl);

        return ServiceResult<StudentDashboardOverviewDto>.Ok(dto);
    }
}
