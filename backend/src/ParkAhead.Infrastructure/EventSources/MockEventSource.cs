using ParkAhead.Application.EventSources;
using ParkAhead.Application.RiskForecast;
using ParkAhead.Domain.Enums;
using ParkAhead.Domain.Events;

namespace ParkAhead.Infrastructure.EventSources;

/// <summary>
/// Temporary stand-in for a real event provider (Ticketmaster, etc.) while the risk-forecast
/// pipeline is being built. Returns a small fixed set of realistic-looking events around Tel
/// Aviv, filtered by the same radius/time-window contract a real provider would honor. Swapping
/// this for a real <see cref="IEventSource"/> implementation is the only change needed to go
/// live — nothing in <see cref="RiskForecastService"/> or <see cref="ParkingRiskEngine"/> knows
/// this is mock data.
/// </summary>
public class MockEventSource : IEventSource
{
    public string Name => "Mock";

    public Task<IReadOnlyList<Event>> GetUpcomingEventsAsync(
        double latitude,
        double longitude,
        double radiusMeters,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        var matching = BuildSampleEvents(now)
            .Where(e => e.StartTime >= from && e.StartTime <= to)
            .Where(e => DistanceCalculator.CalculateKilometers(latitude, longitude, e.Latitude, e.Longitude) * 1000 <= radiusMeters)
            .ToList();

        return Task.FromResult<IReadOnlyList<Event>>(matching);
    }

    // Times are computed relative to "now" on every call (not baked in as static data) so the
    // sample set stays realistic ("starts in 3 hours") for as long as the app keeps running.
    private static List<Event> BuildSampleEvents(DateTimeOffset now) =>
    [
        new()
        {
            Id = "mock-evt-1",
            Title = "Old City Summer Concert",
            VenueName = "Charles Clore Park",
            Latitude = 32.0575,
            Longitude = 34.7638,
            StartTime = now.AddHours(3),
            Category = EventCategory.Concert,
            EstimatedAttendance = 9000
        },
        new()
        {
            Id = "mock-evt-2",
            Title = "Habima Jazz Evening",
            VenueName = "Habima Square",
            Latitude = 32.0748,
            Longitude = 34.7746,
            StartTime = now.AddDays(1),
            Category = EventCategory.Concert,
            EstimatedAttendance = 1200
        },
        new()
        {
            Id = "mock-evt-3",
            Title = "Rabin Square Gathering",
            VenueName = "Rabin Square",
            Latitude = 32.0809,
            Longitude = 34.7806,
            StartTime = now.AddDays(2),
            Category = EventCategory.Other,
            EstimatedAttendance = null
        },
        new()
        {
            Id = "mock-evt-4",
            Title = "Startup Nation Conference",
            VenueName = "Expo Tel Aviv",
            Latitude = 32.0975,
            Longitude = 34.7742,
            StartTime = now.AddDays(5),
            Category = EventCategory.Conference,
            EstimatedAttendance = 4000
        },
        new()
        {
            Id = "mock-evt-5",
            Title = "Riverside Food Festival",
            VenueName = "HaYarkon Park",
            Latitude = 32.0925,
            Longitude = 34.7859,
            StartTime = now.AddDays(6),
            Category = EventCategory.Festival,
            EstimatedAttendance = 15000
        },
        new()
        {
            // ~6 km from central Tel Aviv coordinates — deliberately outside the default 5 km
            // event-search radius, so this large/soon event is correctly excluded and never
            // reaches the risk engine. Proves the source-level radius filter actually works.
            Id = "mock-evt-6",
            Title = "Basketball Championship Night",
            VenueName = "Menora Mivtachim Arena",
            Latitude = 32.114,
            Longitude = 34.803,
            StartTime = now.AddHours(8),
            Category = EventCategory.Sports,
            EstimatedAttendance = 11000
        },
        new()
        {
            // Close by, but 10 days out — deliberately outside the default 7-day forecast
            // window, proving the source-level time filter works independently of distance.
            Id = "mock-evt-7",
            Title = "Neighborhood Art Market",
            VenueName = "Sarona Market",
            Latitude = 32.0714,
            Longitude = 34.7847,
            StartTime = now.AddDays(10),
            Category = EventCategory.Other,
            EstimatedAttendance = 400
        }
    ];
}
