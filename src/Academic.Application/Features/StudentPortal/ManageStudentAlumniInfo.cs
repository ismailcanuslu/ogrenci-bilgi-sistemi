using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;
using Academic.Application.Abstractions;
using Academic.Domain.Students;

namespace Academic.Application.Features.StudentPortal;

public sealed record GetStudentAlumniInfoQuery : IRequest<ServiceResult<StudentAlumniInfoDto>>;

public sealed record UpdateStudentAlumniInfoCommand(
    int? GraduationYear,
    string? CurrentCompany,
    string? CurrentPosition,
    string? LinkedInUrl,
    string? PersonalEmail,
    string? PhoneNumber,
    bool IsVisibleToOtherAlumni) : IRequest<ServiceResult<StudentAlumniInfoDto>>;

public sealed record StudentAlumniInfoDto(
    Guid StudentId,
    int? GraduationYear,
    string? CurrentCompany,
    string? CurrentPosition,
    string? LinkedInUrl,
    string? PersonalEmail,
    string? PhoneNumber,
    bool IsVisibleToOtherAlumni,
    DateTime UpdatedAtUtc);

public sealed class ManageStudentAlumniInfoHandler(
    IStudentPortalRepository portalRepository,
    IAcademicUnitOfWork unitOfWork,
    ICurrentUser currentUser) :
    IRequestHandler<GetStudentAlumniInfoQuery, ServiceResult<StudentAlumniInfoDto>>,
    IRequestHandler<UpdateStudentAlumniInfoCommand, ServiceResult<StudentAlumniInfoDto>>
{
    public async Task<ServiceResult<StudentAlumniInfoDto>> Handle(
        GetStudentAlumniInfoQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<StudentAlumniInfoDto>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<StudentAlumniInfoDto>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        var alumni = await portalRepository.GetAlumniInfoAsync(student.Id, cancellationToken);
        if (alumni is null)
        {
            return ServiceResult<StudentAlumniInfoDto>.Ok(new StudentAlumniInfoDto(
                student.Id,
                null,
                null,
                null,
                null,
                student.Email,
                null,
                true,
                DateTime.UtcNow));
        }

        return ServiceResult<StudentAlumniInfoDto>.Ok(new StudentAlumniInfoDto(
            alumni.StudentId,
            alumni.GraduationYear,
            alumni.CurrentCompany,
            alumni.CurrentPosition,
            alumni.LinkedInUrl,
            alumni.PersonalEmail,
            alumni.PhoneNumber,
            alumni.IsVisibleToOtherAlumni,
            alumni.UpdatedAtUtc));
    }

    public async Task<ServiceResult<StudentAlumniInfoDto>> Handle(
        UpdateStudentAlumniInfoCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<StudentAlumniInfoDto>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<StudentAlumniInfoDto>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        var alumni = new StudentAlumniInfo(
            Guid.NewGuid(),
            student.Id,
            request.GraduationYear,
            request.CurrentCompany,
            request.CurrentPosition,
            request.LinkedInUrl,
            request.PersonalEmail,
            request.PhoneNumber,
            request.IsVisibleToOtherAlumni,
            DateTime.UtcNow);

        await portalRepository.SaveAlumniInfoAsync(alumni, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<StudentAlumniInfoDto>.Ok(new StudentAlumniInfoDto(
            alumni.StudentId,
            alumni.GraduationYear,
            alumni.CurrentCompany,
            alumni.CurrentPosition,
            alumni.LinkedInUrl,
            alumni.PersonalEmail,
            alumni.PhoneNumber,
            alumni.IsVisibleToOtherAlumni,
            alumni.UpdatedAtUtc));
    }
}
