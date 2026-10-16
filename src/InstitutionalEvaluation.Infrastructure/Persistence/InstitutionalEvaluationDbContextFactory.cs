using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InstitutionalEvaluation.Infrastructure.Persistence;

internal sealed class InstitutionalEvaluationDbContextFactory : IDesignTimeDbContextFactory<InstitutionalEvaluationDbContext>
{
    public InstitutionalEvaluationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<InstitutionalEvaluationDbContext>()
            .UseSqlServer("Server=localhost;Database=obs_institutional_evaluation;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        return new InstitutionalEvaluationDbContext(options);
    }
}
