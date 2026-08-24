namespace ParkAhead.Domain.Entities;

/// <summary>
/// A location a user wants parking-risk forecasts for, e.g. their home neighborhood.
/// </summary>
public class MonitoredArea
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    /// <summary>Events within this radius are considered relevant to this area.</summary>
    public double RadiusMeters { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
