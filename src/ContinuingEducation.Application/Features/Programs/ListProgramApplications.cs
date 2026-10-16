using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using ContinuingEducation.Application.Abstractions;
using MediatR;

namespace ContinuingEducation.Application.Features.Programs;

public sealed record ListProgramApplicationsQuery(
    Guid? ProgramId = null,
    bool OnlyMine = true) : IRequest<ServiceResult<IReadOnlyCollection<ProgramApplicationDto>>>;

public sealed record ProgramApplicationDto(
    Guid Id,
    Guid ProgramId,
    Guid ApplicantUserId,
    string Status,
    int AttendedHours,
    decimal? FinalGrade,
    bool IsCertificateEarned,
    string? CertificateNumber,
    DateTime CreatedAtUtc,
    DateTime? ReviewedAtUtc);

public sealed class ListProgramApplicationsHandler(
    IProgramApplicationRepository repository,
    ICurrentUser currentUser,
    IPermissionGuard permissionGuard) : IRequestHandler<ListProgramApplicationsQuery, ServiceResult<IReadOnlyCollection<ProgramApplicationDto>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<ProgramApplicationDto>>> Handle(
        ListProgramApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        Guid? applicantFilter = null;

        if (request.OnlyMine)
        {
            if (currentUser.UserId is not { } userId || userId == Guid.Empty)
            {
                return ServiceResult<IReadOnlyCollection<ProgramApplicationDto>>.Unauthorized(
                    new Error("Auth.Unauthorized", ErrorType.Unauthorized));
            }

            applicantFilter = userId;
        }
        else
        {
            var authError = await permissionGuard.CheckAsync(
                Permissions.ContinuingEducation.ProgramRead,
                [ResourceScope.University],
                cancellationToken);

            if (authError is not null)
            {
                return AuthorizationFailure.To<IReadOnlyCollection<ProgramApplicationDto>>(authError);
            }
        }

        var apps = await repository.ListAsync(request.ProgramId, applicantFilter, cancellationToken);

        var dtos = apps.Select(a => new ProgramApplicationDto(
            a.Id,
            a.ProgramId,
            a.ApplicantUserId,
            a.Status.ToString(),
            a.AttendedHours,
            a.FinalGrade,
            a.IsCertificateEarned,
            a.CertificateNumber,
            a.CreatedAtUtc,
            a.ReviewedAtUtc
        )).ToList();

        return ServiceResult<IReadOnlyCollection<ProgramApplicationDto>>.Ok(dtos);
    }
}
