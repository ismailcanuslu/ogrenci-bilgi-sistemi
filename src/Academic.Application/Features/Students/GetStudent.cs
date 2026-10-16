using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Students;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;

namespace Academic.Application.Features.Students;

public sealed record StudentProgramEnrollmentResponse(
    Guid Id,
    Guid DepartmentId,
    Guid ProgramId,
    Guid CurriculumVersionId,
    EnrollmentType EnrollmentType,
    StudentStatus Status,
    int EnrollmentYear,
    DateTime EnrollmentDateUtc,
    DateTime? CompletionDateUtc,
    bool IsPrimary,
    string? Notes);

public sealed record StudentResponse(
    Guid Id,
    string StudentNumber,
    string FirstName,
    string LastName,
    string Email,
    Guid? UserId,
    Guid DepartmentId,
    Guid ProgramId,
    Guid CurriculumVersionId,
    int EnrollmentYear,
    StudentStatus Status,
    DateTime EnrollmentDateUtc,
    IReadOnlyCollection<StudentProgramEnrollmentResponse> ProgramEnrollments);

public sealed record GetStudentQuery(Guid Id) : IRequest<ServiceResult<StudentResponse>>;

public sealed class GetStudentHandler(
    IStudentRepository studentRepository,
    IDepartmentRepository departmentRepository,
    AcademicAccess access) : IRequestHandler<GetStudentQuery, ServiceResult<StudentResponse>>
{
    public async Task<ServiceResult<StudentResponse>> Handle(GetStudentQuery request, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(request.Id, cancellationToken);
        var department = student is null
            ? null
            : await departmentRepository.GetByIdAsync(student.DepartmentId, cancellationToken);

        var authError = await access.CheckStudentAsync(
            Permissions.Student.Read,
            student?.UserId,
            department?.Id,
            department?.FacultyId,
            student is not null,
            cancellationToken);

        if (authError is not null)
        {
            return AuthorizationFailure.To<StudentResponse>(authError);
        }

        if (student is null)
        {
            return ServiceResult<StudentResponse>.NotFound(AcademicErrors.Student.NotFound());
        }

        var enrollments = student.ProgramEnrollments.Select(p => new StudentProgramEnrollmentResponse(
            p.Id,
            p.DepartmentId,
            p.ProgramId,
            p.CurriculumVersionId,
            p.EnrollmentType,
            p.Status,
            p.EnrollmentYear,
            p.EnrollmentDateUtc,
            p.CompletionDateUtc,
            p.IsPrimary,
            p.Notes)).ToList();

        return ServiceResult<StudentResponse>.Ok(new StudentResponse(
            student.Id,
            student.StudentNumber,
            student.FirstName,
            student.LastName,
            student.Email,
            student.UserId,
            student.DepartmentId,
            student.ProgramId,
            student.CurriculumVersionId,
            student.EnrollmentYear,
            student.Status,
            student.EnrollmentDateUtc,
            enrollments));
    }
}
