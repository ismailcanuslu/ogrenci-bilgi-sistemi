using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CampusLife.Infrastructure.Persistence;

internal sealed class CampusLifeDbContextFactory : IDesignTimeDbContextFactory<CampusLifeDbContext>
{
    public CampusLifeDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<CampusLifeDbContext>()
            .UseSqlServer("Server=localhost;Database=obs_campus_life;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        return new CampusLifeDbContext(options);
    }
}
