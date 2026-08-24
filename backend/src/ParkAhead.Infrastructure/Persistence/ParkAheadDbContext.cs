using Microsoft.EntityFrameworkCore;
using ParkAhead.Domain.Entities;

namespace ParkAhead.Infrastructure.Persistence;

public class ParkAheadDbContext : DbContext
{
    public ParkAheadDbContext(DbContextOptions<ParkAheadDbContext> options) : base(options)
    {
    }

    public DbSet<Event> Events => Set<Event>();

    public DbSet<MonitoredArea> MonitoredAreas => Set<MonitoredArea>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ParkAheadDbContext).Assembly);
    }
}
