using Academic.Application.Abstractions;
using Academic.Application.Common;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;

namespace Academic.Application.Features.CourseOfferings;

public sealed record ListCourseOfferingsQuery(
    Guid? AcademicTermId,
    Guid? CourseId,
    Guid? InstructorId,
    Guid? DepartmentId,
    bool? IsRegistrationOpen,
    int Page = 1,
    int PageSize = 20) : IRequest<ServiceResult<PagedResult<CourseOfferingResponse>>>;

public sealed class ListCourseOfferingsHandler(
    ICourseOfferingRepository offeringRepository,
    AcademicAccess access) : IRequestHandler<ListCourseOfferingsQuery, ServiceResult<PagedResult<CourseOfferingResponse>>>
{
    public async Task<ServiceResult<PagedResult<CourseOfferingResponse>>> Handle(
        ListCourseOfferingsQuery request, CancellationToken cancellationToken)
    {
        var listAccess = await access.GetListAccessAsync(Permissions.Course.Read, cancellationToken);
        if (listAccess.Denied is not null)
        {
            return AuthorizationFailure.To<PagedResult<CourseOfferingResponse>>(listAccess.Denied);
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
                return ServiceResult<PagedResult<CourseOfferingResponse>>.Ok(
                    new PagedResult<CourseOfferingResponse>([], request.Page, request.PageSize, 0));
            }

            departmentFilter = [request.DepartmentId.Value];
        }

        var criteria = new CourseOfferingListCriteria(
            request.AcademicTermId,
            request.CourseId,
            request.InstructorId,
            departmentFilter,
            request.IsRegistrationOpen,
            request.Page,
            request.PageSize);

        var paged = await offeringRepository.ListAsync(criteria, cancellationToken);

        var dtos = paged.Items.Select(o => new CourseOfferingResponse(
            o.Id,
            o.CourseId,
            o.AcademicTermId,
            o.Section,
            o.Capacity,
            o.EnrolledCount,
            o.PrimaryInstructorId,
            o.IsRegistrationOpen,
            o.Instructors.Select(i => new CourseOfferingInstructorResponse(
                i.Id,
                i.InstructorId,
                i.IsPrimary,
                i.RoleOrTitle,
                i.AssignedAtUtc)).ToList(),
            o.Schedules.Select(s => new CourseOfferingScheduleResponse(
                s.Id,
                s.SessionType,
                s.BuildingOrFaculty,
                s.Classroom,
                s.DayOfWeek,
                s.StartTime,
                s.EndTime,
                s.InstructorId,
                s.IsOnline)).ToList())).ToList();

        return ServiceResult<PagedResult<CourseOfferingResponse>>.Ok(
            new PagedResult<CourseOfferingResponse>(dtos, paged.Page, paged.PageSize, paged.TotalCount));
    }
}
