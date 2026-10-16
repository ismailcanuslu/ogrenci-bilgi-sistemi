using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ContinuingEducation.Infrastructure.Persistence;

public sealed class ContinuingEducationDbContextFactory : IDesignTimeDbContextFactory<ContinuingEducationDbContext>
{
    public ContinuingEducationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ContinuingEducationDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=UniversalUniversity;Trusted_Connection=True;TrustServerCertificate=True;");

        return new ContinuingEducationDbContext(optionsBuilder.Options);
    }
}
