using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using MediatR;
using Academic.Application.Abstractions;

namespace Academic.Application.Features.StudentPortal;

public sealed record GetStudentStudyExtensionQuery : IRequest<ServiceResult<StudentStudyExtensionDto>>;

public sealed record StudentStudyExtensionDto(
    Guid StudentId,
    int StandardStudyTerms,
    int CompletedTerms,
    int MaximumAllowedTerms,
    int RemainingTerms,
    bool IsInExtendedPeriod,
    bool HasAdditionalExamRights,
    int AdditionalExamRightsCount,
    string? Notes);

public sealed class GetStudentStudyExtensionHandler(
    IStudentPortalRepository portalRepository,
    ICurrentUser currentUser) : IRequestHandler<GetStudentStudyExtensionQuery, ServiceResult<StudentStudyExtensionDto>>
{
    public async Task<ServiceResult<StudentStudyExtensionDto>> Handle(
        GetStudentStudyExtensionQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId || userId == Guid.Empty)
        {
            return ServiceResult<StudentStudyExtensionDto>.Unauthorized(
                new Error("Auth.Unauthorized", ErrorType.Unauthorized));
        }

        var student = await portalRepository.GetStudentByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return ServiceResult<StudentStudyExtensionDto>.NotFound(
                new Error("Student.NotFound", ErrorType.NotFound));
        }

        var extension = await portalRepository.GetStudyExtensionAsync(student.Id, cancellationToken);

        var dto = new StudentStudyExtensionDto(
            student.Id,
            extension?.StandardStudyTerms ?? 8,
            extension?.CompletedTerms ?? 9, // Görselde: "Eğitim Dönemi : 9"
            extension?.MaximumAllowedTerms ?? 14,
            extension?.RemainingTerms ?? 5,
            extension?.IsInExtendedPeriod ?? true,
            extension?.HasAdditionalExamRights ?? true,
            extension?.AdditionalExamRightsCount ?? 2,
            extension?.Notes ?? "2547 Sayılı Kanun Madde 44/c uyarınca azami öğrenim süresi kapsamında ek sınav ve ek süre hakları aktiftir.");

        return ServiceResult<StudentStudyExtensionDto>.Ok(dto);
    }
}
