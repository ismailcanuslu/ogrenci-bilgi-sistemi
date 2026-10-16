using BuildingBlocks.Application.Abstractions.Security;
using BuildingBlocks.Application.Results;
using DocumentManagement.Application.Abstractions;
using DocumentManagement.Domain.Documents;
using MediatR;

namespace DocumentManagement.Application.Features.Documents;

public sealed record ListDocumentsQuery(
    DocumentType? Type = null,
    DocumentStatus? Status = null,
    bool OnlyMine = false) : IRequest<ServiceResult<IReadOnlyCollection<DocumentSummaryDto>>>;

public sealed record DocumentSummaryDto(
    Guid Id,
    string? DocumentNumber,
    string Subject,
    string DocumentType,
    string Status,
    Guid CreatorUserId,
    string? DepartmentCode,
    string VerificationCode,
    DateTime CreatedAtUtc,
    DateTime? SignedAtUtc);

public sealed class ListDocumentsHandler(
    IOfficialDocumentRepository documentRepository,
    ICurrentUser currentUser,
    IPermissionGuard permissionGuard) : IRequestHandler<ListDocumentsQuery, ServiceResult<IReadOnlyCollection<DocumentSummaryDto>>>
{
    public async Task<ServiceResult<IReadOnlyCollection<DocumentSummaryDto>>> Handle(
        ListDocumentsQuery request,
        CancellationToken cancellationToken)
    {
        Guid? creatorFilter = null;

        if (request.OnlyMine)
        {
            if (currentUser.UserId is not { } userId || userId == Guid.Empty)
            {
                return ServiceResult<IReadOnlyCollection<DocumentSummaryDto>>.Unauthorized(
                    new Error("Auth.Unauthorized", ErrorType.Unauthorized));
            }

            creatorFilter = userId;
        }
        else
        {
            var authError = await permissionGuard.CheckAsync(
                Permissions.DocumentManagement.DocumentManage,
                [ResourceScope.University],
                cancellationToken);

            if (authError is not null)
            {
                return AuthorizationFailure.To<IReadOnlyCollection<DocumentSummaryDto>>(authError);
            }
        }

        var docs = await documentRepository.ListAsync(request.Type, request.Status, creatorFilter, cancellationToken);

        var dtos = docs.Select(d => new DocumentSummaryDto(
            d.Id,
            d.DocumentNumber,
            d.Subject,
            d.Type.ToString(),
            d.Status.ToString(),
            d.CreatorUserId,
            d.DepartmentCode,
            d.VerificationCode,
            d.CreatedAtUtc,
            d.SignedAtUtc
        )).ToList();

        return ServiceResult<IReadOnlyCollection<DocumentSummaryDto>>.Ok(dtos);
    }
}
