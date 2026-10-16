using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Security.Infrastructure.Persistence;

internal sealed class SecurityDbContextFactory : IDesignTimeDbContextFactory<SecurityDbContext>
{
    public SecurityDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SecurityDbContext>()
            .UseSqlServer("Server=localhost;Database=obs_security;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        return new SecurityDbContext(options);
    }
}
