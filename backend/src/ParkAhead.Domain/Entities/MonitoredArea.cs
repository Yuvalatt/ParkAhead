using ParkAhead.Domain.Enums;

namespace ParkAhead.Domain.Entities;

/// <summary>
/// A location a user wants parking-risk forecasts for, e.g. their home neighborhood.
/// </summary>
public class MonitoredArea
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public AreaType AreaType { get; set; }

    /// <summary>Formatted address as resolved by the geocoding source (e.g. Google Places) at creation time.</summary>
    public string Address { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    /// <summary>Events within this radius are considered relevant to this area.</summary>
    public double RadiusMeters { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
