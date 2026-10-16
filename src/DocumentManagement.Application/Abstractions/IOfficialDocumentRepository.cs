using DocumentManagement.Domain.Documents;

namespace DocumentManagement.Application.Abstractions;

public interface IOfficialDocumentRepository
{
    Task<OfficialDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OfficialDocument?> GetByVerificationCodeAsync(string verificationCode, CancellationToken cancellationToken = default);
    Task<OfficialDocument?> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<OfficialDocument>> ListAsync(
        DocumentType? type = null,
        DocumentStatus? status = null,
        Guid? creatorUserId = null,
        CancellationToken cancellationToken = default);
    Task AddAsync(OfficialDocument document, CancellationToken cancellationToken = default);
}
