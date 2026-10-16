using BuildingBlocks.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using File = Files.Domain.Files.File;

namespace Files.Infrastructure.Persistence;

public sealed class FilesDbContext : DbContext, IUnitOfWork
{
    public FilesDbContext(DbContextOptions<FilesDbContext> options)
        : base(options)
    {
    }

    public DbSet<File> Files => Set<File>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FilesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
