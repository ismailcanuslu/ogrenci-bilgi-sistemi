using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;
using Academic.Application.Abstractions;

namespace Academic.Application.Features.StudentPortal;

public sealed record GetStudentAdvisorQuery : IRequest<ServiceResult<StudentAdvisorDto>>;

public sealed record StudentAdvisorDto(
    Guid? AdvisorInstructorId,
    string Title,
    string FullName,
    string Department,
    string Email,
    string OfficeLocation,
    string OfficeHours,
    DateTime? AssignedDateUtc);

public sealed class GetStudentAdvisorHandler(
    IStudentPortalRepository portalRepository,
    IDepartmentRepository departmentRepository,
    ICurrentUser currentUser) : IRequestHandler<GetStudentAdvisorQuery, ServiceResult<StudentAdvisorDto>>
{
    public async Task<ServiceResult<StudentAdvisorDto>> Handle(
        GetStudentAdvisorQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<StudentAdvisorDto>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<StudentAdvisorDto>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        var assignment = await portalRepository.GetActiveAdvisorAssignmentAsync(student.Id, cancellationToken);
        var dept = await departmentRepository.GetByIdAsync(student.DepartmentId, cancellationToken);

        var dto = new StudentAdvisorDto(
            assignment?.InstructorId,
            "Doç. Dr.",
            "Akademik Danışman",
            dept?.Name ?? "Bilgisayar Mühendisliği",
            "danisman@university.edu.tr",
            "Mühendislik Fakültesi B-Blok Kat:3 No:312",
            "Salı & Perşembe 14:00 - 16:00",
            assignment?.AssignedDateUtc);

        return ServiceResult<StudentAdvisorDto>.Ok(dto);
    }
}
