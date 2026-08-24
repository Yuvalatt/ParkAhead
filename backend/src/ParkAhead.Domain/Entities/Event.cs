namespace ParkAhead.Domain.Entities;

/// <summary>
/// A normalized event pulled from an external provider (see ParkAhead.Application.EventSources.IEventSource).
/// Provider-specific response shapes are mapped into this type before anything else in the app sees them.
/// </summary>
public class Event
{
    public Guid Id { get; set; }

    /// <summary>Id of this event as assigned by the external provider.</summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>Name of the provider this event came from, e.g. "Ticketmaster" or "Mock".</summary>
    public string Source { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public DateTimeOffset StartDateTime { get; set; }

    public DateTimeOffset? EndDateTime { get; set; }

    public string? VenueName { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Category { get; set; }

    /// <summary>Attendance estimate when the provider offers one (or can be derived from venue/classification).</summary>
    public int? EstimatedAttendance { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
