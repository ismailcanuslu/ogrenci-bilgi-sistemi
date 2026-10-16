using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SocialContribution.Infrastructure.Persistence;

internal sealed class SocialContributionDbContextFactory : IDesignTimeDbContextFactory<SocialContributionDbContext>
{
    public SocialContributionDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SocialContributionDbContext>()
            .UseSqlServer("Server=localhost;Database=obs_social_contribution;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        return new SocialContributionDbContext(options);
    }
}
