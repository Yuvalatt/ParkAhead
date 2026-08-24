using ParkAhead.Domain.Entities;

namespace ParkAhead.Application.EventSources;

/// <summary>
/// Abstraction over an external event provider. Implementations live in ParkAhead.Infrastructure and are
/// responsible for translating provider-specific responses into <see cref="Event"/> before returning them —
/// no provider-shaped data crosses this boundary.
/// </summary>
public interface IEventSource
{
    /// <summary>Provider name, e.g. "Ticketmaster" or "Mock". Stored on <see cref="Event.Source"/>.</summary>
    string Name { get; }

    Task<IReadOnlyList<Event>> GetUpcomingEventsAsync(
        double latitude,
        double longitude,
        double radiusMeters,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken);
}
