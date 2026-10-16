using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using FluentValidation;
using MediatR;
using Academic.Application.Abstractions;
using Academic.Domain.Students;

namespace Academic.Application.Features.StudentPortal;

public sealed record GetStudentProfilePictureQuery : IRequest<ServiceResult<StudentProfilePictureDto>>;

public sealed record UploadStudentProfilePictureCommand(string ImageUrl) : IRequest<ServiceResult<StudentProfilePictureDto>>;

public sealed record StudentProfilePictureDto(
    Guid StudentId,
    string ImageUrl,
    string Status,
    DateTime UploadedAtUtc,
    DateTime? ReviewedAtUtc,
    string? RejectionReason);

public sealed class UploadStudentProfilePictureValidator : AbstractValidator<UploadStudentProfilePictureCommand>
{
    public UploadStudentProfilePictureValidator()
    {
        RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(StudentProfilePicture.ImageUrlMaxLength);
    }
}

public sealed class ManageStudentProfilePictureHandler(
    IStudentPortalRepository portalRepository,
    IAcademicUnitOfWork unitOfWork,
    ICurrentUser currentUser) :
    IRequestHandler<GetStudentProfilePictureQuery, ServiceResult<StudentProfilePictureDto>>,
    IRequestHandler<UploadStudentProfilePictureCommand, ServiceResult<StudentProfilePictureDto>>
{
    public async Task<ServiceResult<StudentProfilePictureDto>> Handle(
        GetStudentProfilePictureQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<StudentProfilePictureDto>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<StudentProfilePictureDto>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        var pic = await portalRepository.GetProfilePictureAsync(student.Id, cancellationToken);
        if (pic is null)
        {
            return ServiceResult<StudentProfilePictureDto>.NotFound(
                new Error("StudentProfilePicture.NotFound", ErrorType.NotFound));
        }

        return ServiceResult<StudentProfilePictureDto>.Ok(new StudentProfilePictureDto(
            pic.StudentId,
            pic.ImageUrl,
            pic.Status.ToString(),
            pic.UploadedAtUtc,
            pic.ReviewedAtUtc,
            pic.RejectionReason));
    }

    public async Task<ServiceResult<StudentProfilePictureDto>> Handle(
        UploadStudentProfilePictureCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<StudentProfilePictureDto>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<StudentProfilePictureDto>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        var newPic = new StudentProfilePicture(Guid.NewGuid(), student.Id, request.ImageUrl, DateTime.UtcNow);
        await portalRepository.SaveProfilePictureAsync(newPic, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<StudentProfilePictureDto>.Ok(new StudentProfilePictureDto(
            newPic.StudentId,
            newPic.ImageUrl,
            newPic.Status.ToString(),
            newPic.UploadedAtUtc,
            newPic.ReviewedAtUtc,
            newPic.RejectionReason));
    }
}
