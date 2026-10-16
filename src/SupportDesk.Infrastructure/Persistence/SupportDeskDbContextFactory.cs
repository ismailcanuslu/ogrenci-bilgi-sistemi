using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SupportDesk.Infrastructure.Persistence;

internal sealed class SupportDeskDbContextFactory : IDesignTimeDbContextFactory<SupportDeskDbContext>
{
    public SupportDeskDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SupportDeskDbContext>()
            .UseSqlServer("Server=localhost;Database=obs_support_desk;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        return new SupportDeskDbContext(options);
    }
}
