using Academic.Application.Abstractions;
using Academic.Application.Common;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;

namespace Academic.Application.Features.Courses;

public sealed record CourseResponse(
    Guid Id,
    Guid DepartmentId,
    string Code,
    string Name,
    string? Description,
    int DefaultEcts,
    int DefaultCredit,
    string? Prerequisites,
    bool IsActive);

public sealed record GetCourseQuery(Guid Id) : IRequest<ServiceResult<CourseResponse>>;

public sealed class GetCourseHandler(
    ICourseRepository courseRepository,
    IDepartmentRepository departmentRepository,
    AcademicAccess access) : IRequestHandler<GetCourseQuery, ServiceResult<CourseResponse>>
{
    public async Task<ServiceResult<CourseResponse>> Handle(GetCourseQuery request, CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetByIdAsync(request.Id, cancellationToken);
        var department = course is null
            ? null
            : await departmentRepository.GetByIdAsync(course.DepartmentId, cancellationToken);

        var authError = await access.CheckCourseAsync(
            Permissions.Course.Read,
            department?.Id,
            department?.FacultyId,
            course is not null,
            cancellationToken);

        if (authError is not null)
        {
            return AuthorizationFailure.To<CourseResponse>(authError);
        }

        if (course is null)
        {
            return ServiceResult<CourseResponse>.NotFound(AcademicErrors.Course.NotFound());
        }

        return ServiceResult<CourseResponse>.Ok(new CourseResponse(
            course.Id,
            course.DepartmentId,
            course.Code,
            course.Name,
            course.Description,
            course.DefaultEcts,
            course.DefaultCredit,
            course.Prerequisites,
            course.IsActive));
    }
}
