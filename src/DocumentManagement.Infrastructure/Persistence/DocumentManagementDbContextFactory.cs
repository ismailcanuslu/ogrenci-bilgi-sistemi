using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DocumentManagement.Infrastructure.Persistence;

public sealed class DocumentManagementDbContextFactory : IDesignTimeDbContextFactory<DocumentManagementDbContext>
{
    public DocumentManagementDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DocumentManagementDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=UniversalUniversity;Trusted_Connection=True;TrustServerCertificate=True;");

        return new DocumentManagementDbContext(optionsBuilder.Options);
    }
}
