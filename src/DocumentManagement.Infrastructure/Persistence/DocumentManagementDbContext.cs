using DocumentManagement.Application.Abstractions;
using DocumentManagement.Domain.Documents;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Infrastructure.Persistence;

public sealed class DocumentManagementDbContext : DbContext, IDocumentManagementUnitOfWork
{
    public const string Schema = "document_management";

    public DocumentManagementDbContext(DbContextOptions<DocumentManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<OfficialDocument> OfficialDocuments => Set<OfficialDocument>();

    public DbSet<DocumentWorkflowStep> DocumentWorkflowSteps => Set<DocumentWorkflowStep>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DocumentManagementDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
