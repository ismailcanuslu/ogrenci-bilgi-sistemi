using Microsoft.EntityFrameworkCore;
using SocialContribution.Domain;

namespace SocialContribution.Infrastructure.Persistence;

public class SocialContributionDbContext : DbContext
{
    public SocialContributionDbContext(DbContextOptions<SocialContributionDbContext> options)
        : base(options)
    {
    }

    public DbSet<SocialContributionProject> SocialContributionProjects => Set<SocialContributionProject>();
    public DbSet<ProjectVolunteer> ProjectVolunteers => Set<ProjectVolunteer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SocialContributionProject>(b =>
        {
            b.ToTable("SocialContributionProjects");
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).IsRequired().HasMaxLength(300);
            b.Property(x => x.Description).IsRequired().HasMaxLength(4000);
            b.Property(x => x.CoordinatorName).IsRequired().HasMaxLength(200);
            b.Property(x => x.TargetAudience).IsRequired().HasMaxLength(250);
            b.Property(x => x.PartnerOrganization).IsRequired().HasMaxLength(250);
            b.Property(x => x.RejectionReason).HasMaxLength(1000);
            b.Property(x => x.ImpactSummary).HasMaxLength(4000);

            b.HasMany(x => x.Volunteers)
                .WithOne()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProjectVolunteer>(b =>
        {
            b.ToTable("ProjectVolunteers");
            b.HasKey(x => x.Id);
            b.Property(x => x.StudentOrStaffNo).IsRequired().HasMaxLength(50);
            b.Property(x => x.FullName).IsRequired().HasMaxLength(200);
            b.Property(x => x.Email).IsRequired().HasMaxLength(200);
            b.Property(x => x.Note).HasMaxLength(1000);
        });
    }
}
