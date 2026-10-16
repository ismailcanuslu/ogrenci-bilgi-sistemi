using BuildingBlocks.Application.Results;
using DocumentManagement.Application.Abstractions;
using DocumentManagement.Domain.Documents;
using MediatR;

namespace DocumentManagement.Application.Features.Documents;

public sealed record VerifyDocumentQuery(string VerificationCode) : IRequest<ServiceResult<DocumentVerificationResultDto>>;

public sealed record DocumentVerificationResultDto(
    string VerificationCode,
    string? DocumentNumber,
    string Subject,
    string ContentSummary,
    string DocumentType,
    string Status,
    bool IsValid,
    string? DepartmentCode,
    DateTime CreatedAtUtc,
    DateTime? SignedAtUtc,
    string VerificationHash);

public sealed class VerifyDocumentHandler(
    IOfficialDocumentRepository documentRepository) : IRequestHandler<VerifyDocumentQuery, ServiceResult<DocumentVerificationResultDto>>
{
    public async Task<ServiceResult<DocumentVerificationResultDto>> Handle(
        VerifyDocumentQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.VerificationCode))
        {
            return ServiceResult<DocumentVerificationResultDto>.Failure(
                new Error("DocumentVerification.CodeRequired", ErrorType.Validation));
        }

        var document = await documentRepository.GetByVerificationCodeAsync(
            request.VerificationCode.Trim(),
            cancellationToken);

        if (document is null)
        {
            return ServiceResult<DocumentVerificationResultDto>.NotFound(
                new Error("DocumentVerification.NotFound", ErrorType.NotFound));
        }

        var isValid = document.Status == DocumentStatus.Signed;

        var dto = new DocumentVerificationResultDto(
            document.VerificationCode,
            document.DocumentNumber,
            document.Subject,
            document.ContentSummary,
            document.Type.ToString(),
            document.Status.ToString(),
            isValid,
            document.DepartmentCode,
            document.CreatedAtUtc,
            document.SignedAtUtc,
            document.VerificationHash);

        return ServiceResult<DocumentVerificationResultDto>.Ok(dto);
    }
}
