using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Students;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;

namespace Academic.Application.Features.Students;

public sealed record ListStudentsQuery(
    Guid? DepartmentId,
    Guid? ProgramId,
    StudentStatus? Status,
    int? EnrollmentYear,
    string? Search,
    int Page = 1,
    int PageSize = 20) : IRequest<ServiceResult<PagedResult<StudentResponse>>>;

public sealed class ListStudentsHandler(
    IStudentRepository studentRepository,
    AcademicAccess access) : IRequestHandler<ListStudentsQuery, ServiceResult<PagedResult<StudentResponse>>>
{
    public async Task<ServiceResult<PagedResult<StudentResponse>>> Handle(
        ListStudentsQuery request, CancellationToken cancellationToken)
    {
        var listAccess = await access.GetListAccessAsync(Permissions.Student.Read, cancellationToken);
        if (listAccess.Denied is not null)
        {
            return AuthorizationFailure.To<PagedResult<StudentResponse>>(listAccess.Denied);
        }

        IReadOnlyCollection<Guid>? departmentFilter = null;
        if (!listAccess.Unrestricted)
        {
            departmentFilter = listAccess.DepartmentIds;
        }

        if (request.DepartmentId.HasValue)
        {
            if (departmentFilter is not null && !departmentFilter.Contains(request.DepartmentId.Value))
            {
                return ServiceResult<PagedResult<StudentResponse>>.Ok(
                    new PagedResult<StudentResponse>([], request.Page, request.PageSize, 0));
            }

            departmentFilter = [request.DepartmentId.Value];
        }

        var criteria = new StudentListCriteria(
            departmentFilter,
            request.ProgramId,
            request.Status,
            request.EnrollmentYear,
            request.Search,
            request.Page,
            request.PageSize);

        var paged = await studentRepository.ListAsync(criteria, cancellationToken);

        var dtos = paged.Items.Select(s => new StudentResponse(
            s.Id,
            s.StudentNumber,
            s.FirstName,
            s.LastName,
            s.Email,
            s.UserId,
            s.DepartmentId,
            s.ProgramId,
            s.CurriculumVersionId,
            s.EnrollmentYear,
            s.Status,
            s.EnrollmentDateUtc,
            s.ProgramEnrollments.Select(p => new StudentProgramEnrollmentResponse(
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
                p.Notes)).ToList())).ToList();

        return ServiceResult<PagedResult<StudentResponse>>.Ok(
            new PagedResult<StudentResponse>(dtos, paged.Page, paged.PageSize, paged.TotalCount));
    }
}
