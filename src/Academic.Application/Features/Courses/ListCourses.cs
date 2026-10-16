using Academic.Application.Abstractions;
using Academic.Application.Common;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;

namespace Academic.Application.Features.Courses;

public sealed record ListCoursesQuery(
    Guid? DepartmentId,
    bool? IsActive,
    string? Search,
    int Page = 1,
    int PageSize = 20) : IRequest<ServiceResult<PagedResult<CourseResponse>>>;

public sealed class ListCoursesHandler(
    ICourseRepository courseRepository,
    AcademicAccess access) : IRequestHandler<ListCoursesQuery, ServiceResult<PagedResult<CourseResponse>>>
{
    public async Task<ServiceResult<PagedResult<CourseResponse>>> Handle(
        ListCoursesQuery request, CancellationToken cancellationToken)
    {
        var listAccess = await access.GetListAccessAsync(Permissions.Course.Read, cancellationToken);
        if (listAccess.Denied is not null)
        {
            return AuthorizationFailure.To<PagedResult<CourseResponse>>(listAccess.Denied);
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
                return ServiceResult<PagedResult<CourseResponse>>.Ok(
                    new PagedResult<CourseResponse>([], request.Page, request.PageSize, 0));
            }

            departmentFilter = [request.DepartmentId.Value];
        }

        var criteria = new CourseListCriteria(
            departmentFilter,
            request.IsActive,
            request.Search,
            request.Page,
            request.PageSize);

        var paged = await courseRepository.ListAsync(criteria, cancellationToken);

        var dtos = paged.Items.Select(c => new CourseResponse(
            c.Id,
            c.DepartmentId,
            c.Code,
            c.Name,
            c.Description,
            c.DefaultEcts,
            c.DefaultCredit,
            c.Prerequisites,
            c.IsActive)).ToList();

        return ServiceResult<PagedResult<CourseResponse>>.Ok(
            new PagedResult<CourseResponse>(dtos, paged.Page, paged.PageSize, paged.TotalCount));
    }
}
