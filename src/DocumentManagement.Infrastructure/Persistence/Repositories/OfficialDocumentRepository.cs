using DocumentManagement.Application.Abstractions;
using DocumentManagement.Domain.Documents;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Infrastructure.Persistence.Repositories;

public sealed class OfficialDocumentRepository : IOfficialDocumentRepository
{
    private readonly DocumentManagementDbContext _dbContext;

    public OfficialDocumentRepository(DocumentManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<OfficialDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.OfficialDocuments
            .Include(x => x.WorkflowSteps)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<OfficialDocument?> GetByVerificationCodeAsync(string verificationCode, CancellationToken cancellationToken = default)
    {
        var normalized = verificationCode.Trim();
        return _dbContext.OfficialDocuments
            .Include(x => x.WorkflowSteps)
            .FirstOrDefaultAsync(x => x.VerificationCode == normalized, cancellationToken);
    }

    public Task<OfficialDocument?> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        var normalized = documentNumber.Trim();
        return _dbContext.OfficialDocuments
            .Include(x => x.WorkflowSteps)
            .FirstOrDefaultAsync(x => x.DocumentNumber == normalized, cancellationToken);
    }

    public async Task<IReadOnlyCollection<OfficialDocument>> ListAsync(
        DocumentType? type = null,
        DocumentStatus? status = null,
        Guid? creatorUserId = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<OfficialDocument> query = _dbContext.OfficialDocuments.Include(x => x.WorkflowSteps);

        if (type.HasValue)
        {
            query = query.Where(x => x.Type == type.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (creatorUserId.HasValue)
        {
            query = query.Where(x => x.CreatorUserId == creatorUserId.Value);
        }

        return await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(OfficialDocument document, CancellationToken cancellationToken = default)
    {
        await _dbContext.OfficialDocuments.AddAsync(document, cancellationToken);
    }
}
