using CampusLife.Application.Abstractions;
using CampusLife.Domain.Clubs;
using CampusLife.Domain.Reservations;
using Microsoft.EntityFrameworkCore;

namespace CampusLife.Infrastructure.Persistence;

public sealed class CampusLifeDbContext(DbContextOptions<CampusLifeDbContext> options)
    : DbContext(options), ICampusLifeUnitOfWork
{
    public DbSet<FacilityReservation> FacilityReservations => Set<FacilityReservation>();
    public DbSet<StudentClub> StudentClubs => Set<StudentClub>();
    public DbSet<ClubEvent> ClubEvents => Set<ClubEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("campus_life");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CampusLifeDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
