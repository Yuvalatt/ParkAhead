using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParkAhead.Domain.Entities;

namespace ParkAhead.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ExternalId).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Source).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(500);
        builder.Property(e => e.VenueName).HasMaxLength(300);
        builder.Property(e => e.Category).HasMaxLength(100);

        // Prevents duplicate ingestion of the same provider event across refresh cycles.
        builder.HasIndex(e => new { e.Source, e.ExternalId }).IsUnique();

        // Forecast queries filter upcoming events by date range.
        builder.HasIndex(e => e.StartDateTime);
    }
}
