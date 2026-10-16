using Academic.Application.Abstractions;
using Academic.Application.Common;
using Academic.Domain.Courses;
using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;

namespace Academic.Application.Features.CourseOfferings;

public sealed record CourseOfferingInstructorResponse(
    Guid Id,
    Guid InstructorId,
    bool IsPrimary,
    string? RoleOrTitle,
    DateTime AssignedAtUtc);

public sealed record CourseOfferingScheduleResponse(
    Guid Id,
    SessionType SessionType,
    string BuildingOrFaculty,
    string Classroom,
    DayOfWeek DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    Guid? InstructorId,
    bool IsOnline);

public sealed record CourseOfferingResponse(
    Guid Id,
    Guid CourseId,
    Guid AcademicTermId,
    int Section,
    int Capacity,
    int EnrolledCount,
    Guid? PrimaryInstructorId,
    bool IsRegistrationOpen,
    IReadOnlyCollection<CourseOfferingInstructorResponse> Instructors,
    IReadOnlyCollection<CourseOfferingScheduleResponse> Schedules);

public sealed record GetCourseOfferingQuery(Guid Id) : IRequest<ServiceResult<CourseOfferingResponse>>;

public sealed class GetCourseOfferingHandler(
    ICourseOfferingRepository offeringRepository,
    ICourseRepository courseRepository,
    IDepartmentRepository departmentRepository,
    AcademicAccess access) : IRequestHandler<GetCourseOfferingQuery, ServiceResult<CourseOfferingResponse>>
{
    public async Task<ServiceResult<CourseOfferingResponse>> Handle(
        GetCourseOfferingQuery request, CancellationToken cancellationToken)
    {
        var offering = await offeringRepository.GetByIdAsync(request.Id, true, cancellationToken);
        var course = offering is null
            ? null
            : await courseRepository.GetByIdAsync(offering.CourseId, cancellationToken);
        var department = course is null
            ? null
            : await departmentRepository.GetByIdAsync(course.DepartmentId, cancellationToken);

        var authError = await access.CheckCourseOfferingAsync(
            Permissions.Course.Read,
            offering?.Id ?? Guid.Empty,
            offering?.PrimaryInstructorId,
            department?.Id,
            department?.FacultyId,
            offering is not null,
            cancellationToken);

        if (authError is not null)
        {
            return AuthorizationFailure.To<CourseOfferingResponse>(authError);
        }

        if (offering is null)
        {
            return ServiceResult<CourseOfferingResponse>.NotFound(AcademicErrors.CourseOffering.NotFound());
        }

        var instructors = offering.Instructors.Select(i => new CourseOfferingInstructorResponse(
            i.Id,
            i.InstructorId,
            i.IsPrimary,
            i.RoleOrTitle,
            i.AssignedAtUtc)).ToList();

        var schedules = offering.Schedules.Select(s => new CourseOfferingScheduleResponse(
            s.Id,
            s.SessionType,
            s.BuildingOrFaculty,
            s.Classroom,
            s.DayOfWeek,
            s.StartTime,
            s.EndTime,
            s.InstructorId,
            s.IsOnline)).ToList();

        return ServiceResult<CourseOfferingResponse>.Ok(new CourseOfferingResponse(
            offering.Id,
            offering.CourseId,
            offering.AcademicTermId,
            offering.Section,
            offering.Capacity,
            offering.EnrolledCount,
            offering.PrimaryInstructorId,
            offering.IsRegistrationOpen,
            instructors,
            schedules));
    }
}
