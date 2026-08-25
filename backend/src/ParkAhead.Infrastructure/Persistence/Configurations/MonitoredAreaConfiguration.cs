using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParkAhead.Domain.Entities;

namespace ParkAhead.Infrastructure.Persistence.Configurations;

public class MonitoredAreaConfiguration : IEntityTypeConfiguration<MonitoredArea>
{
    public void Configure(EntityTypeBuilder<MonitoredArea> builder)
    {
        builder.ToTable("monitored_areas");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Address).IsRequired().HasMaxLength(500);

        // Stored as its string name (not a lookup table) — small fixed set of values, and
        // "Home"/"Work"/"Other" in the database is more debuggable than a bare int.
        builder.Property(a => a.AreaType).IsRequired().HasMaxLength(20).HasConversion<string>();
    }
}
