using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StudentApplications.Infrastructure.Persistence;

public sealed class StudentApplicationsDbContextFactory : IDesignTimeDbContextFactory<StudentApplicationsDbContext>
{
    public StudentApplicationsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StudentApplicationsDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=UniversalUniversity;Trusted_Connection=True;TrustServerCertificate=True;");

        return new StudentApplicationsDbContext(optionsBuilder.Options);
    }
}
