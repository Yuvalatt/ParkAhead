using ParkAhead.Domain.Enums;

namespace ParkAhead.Domain.Events;

/// <summary>
/// A normalized event from an external provider, relevant to parking-risk calculation.
/// Provider-agnostic and intentionally not persisted (see IEventSource) — it's fetched fresh
/// on each forecast request rather than stored, so there's nothing here about ingestion,
/// source tracking, or database mapping. A real provider's response shape gets mapped into
/// this type at the boundary; nothing downstream ever sees provider-specific data.
/// </summary>
public class Event
{
    public required string Id { get; init; }

    public required string Title { get; init; }

    public string? VenueName { get; init; }

    public required double Latitude { get; init; }

    public required double Longitude { get; init; }

    public required DateTimeOffset StartTime { get; init; }

    public required EventCategory Category { get; init; }

    public int? EstimatedAttendance { get; init; }
}
