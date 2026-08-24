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
    }
}
