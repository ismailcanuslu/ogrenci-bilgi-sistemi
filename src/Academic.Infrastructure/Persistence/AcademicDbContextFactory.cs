using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Academic.Infrastructure.Persistence;

/// <summary>Yalnızca `dotnet ef` (migration üretimi) için kullanılır; çalışma zamanında devreye girmez.</summary>
internal sealed class AcademicDbContextFactory : IDesignTimeDbContextFactory<AcademicDbContext>
{
    public AcademicDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AcademicDbContext>()
            .UseSqlServer("Server=localhost;Database=obs_academic;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        return new AcademicDbContext(options);
    }
}
