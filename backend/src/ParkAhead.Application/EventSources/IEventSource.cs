using ParkAhead.Domain.Events;

namespace ParkAhead.Application.EventSources;

/// <summary>
/// Abstraction over an external event provider. Implementations live in ParkAhead.Infrastructure and are
/// responsible for translating provider-specific responses into <see cref="Event"/> before returning them —
/// no provider-shaped data crosses this boundary. This is the seam a real provider (Ticketmaster, etc.)
/// plugs into later without the risk-forecast pipeline downstream ever changing.
/// </summary>
public interface IEventSource
{
    /// <summary>Provider name, e.g. "Ticketmaster" or "Mock".</summary>
    string Name { get; }

    Task<IReadOnlyList<Event>> GetUpcomingEventsAsync(
        double latitude,
        double longitude,
        double radiusMeters,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken);
}
