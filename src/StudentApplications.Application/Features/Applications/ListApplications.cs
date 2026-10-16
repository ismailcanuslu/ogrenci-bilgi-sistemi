using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;
using StudentApplications.Application.Abstractions;
using StudentApplications.Domain.Applications;

namespace StudentApplications.Application.Features.Applications;

public sealed record ListApplicationsQuery(
    ApplicationType? Type = null,
    ApplicationState? State = null,
    Guid? TargetDepartmentId = null,
    bool OnlyMine = true) : IRequest<ServiceResult<IReadOnlyCollection<StudentApplicationDto>>>;

public sealed record StudentApplicationDto(
    Guid Id,
    string ApplicationNumber,
    Guid StudentId,
    string Type,
    string State,
    Guid? TargetDepartmentId,
    string? TargetInstitution,
    string Details,
    string? AttachmentUrl,
    decimal? Gpa,
    string? ReviewNotes,
    Guid? ReviewerUserId,
    DateTime CreatedAtUtc,
    DateTime? ReviewedAtUtc);

public sealed class ListApplicationsHandler(
    IStudentApplicationRepository applicationRepository,
    ICurrentUser currentUser,
    IPermissionGuard permissionGuard) : IRequestHandler<ListApplicationsQuery, ServiceResult<IReadOnlyCollection<StudentApplicationDto>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<StudentApplicationDto>>> Handle(
        ListApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        Guid? studentFilter = null;

        if (request.OnlyMine)
        {
            if (currentUser.UserId is not { } studentId || studentId == Guid.Empty)
            {
                return ServiceResult<IReadOnlyCollection<StudentApplicationDto>>.Unauthorized(
                    new Error("Auth.Unauthorized", ErrorType.Unauthorized));
            }

            studentFilter = studentId;
        }
        else
        {
            var authError = await permissionGuard.CheckAsync(
                Permissions.Student.EnrollmentManage,
                [ResourceScope.University],
                cancellationToken);

            if (authError is not null)
            {
                return AuthorizationFailure.To<IReadOnlyCollection<StudentApplicationDto>>(authError);
            }
        }

        var apps = await applicationRepository.ListAsync(
            studentFilter,
            request.Type,
            request.State,
            request.TargetDepartmentId,
            cancellationToken);

        var dtos = apps.Select(a => new StudentApplicationDto(
            a.Id,
            a.ApplicationNumber,
            a.StudentId,
            a.Type.ToString(),
            a.State.ToString(),
            a.TargetDepartmentId,
            a.TargetInstitution,
            a.Details,
            a.AttachmentUrl,
            a.Gpa,
            a.ReviewNotes,
            a.ReviewerUserId,
            a.CreatedAtUtc,
            a.ReviewedAtUtc
        )).ToList();

        return ServiceResult<IReadOnlyCollection<StudentApplicationDto>>.Ok(dtos);
    }
}
