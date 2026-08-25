using ParkAhead.Application.EventSources;
using ParkAhead.Application.MonitoredAreas;
using ParkAhead.Domain.Entities;
using ParkAhead.Domain.Enums;

namespace ParkAhead.Application.RiskForecast;

/// <summary>
/// Orchestrates one forecast: fetch nearby upcoming events via <see cref="IEventSource"/>, score
/// each with <see cref="ParkingRiskEngine"/>, and shape the API response. Deliberately knows
/// nothing about persistence — the caller (the controller) looks up the MonitoredArea and hands
/// it in, so this class stays a pure orchestration layer over the event source + risk engine.
/// </summary>
public class RiskForecastService
{
    // Independent of the monitored area's own radius: a large event several km outside the area
    // a user cares about can still affect its parking, so event discovery always searches this
    // fixed radius around the area's coordinates rather than the (typically much smaller) radius
    // the user picked for "how close counts as my area".
    private const double EventSearchRadiusMeters = 5_000; // 5 km

    private const int ForecastWindowDays = 7;

    private readonly IEventSource _eventSource;

    public RiskForecastService(IEventSource eventSource)
    {
        _eventSource = eventSource;
    }

    public async Task<RiskForecastResponse> GenerateForecastAsync(MonitoredArea area, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        var events = await _eventSource.GetUpcomingEventsAsync(
            area.Latitude,
            area.Longitude,
            EventSearchRadiusMeters,
            now,
            now.AddDays(ForecastWindowDays),
            cancellationToken);

        var assessments = events
            .Select(e => ParkingRiskEngine.Assess(e, area.Latitude, area.Longitude, now))
            .OrderByDescending(a => a.Score)
            .ToList();

        var summary = new RiskForecastSummary(
            UpcomingEventCount: assessments.Count,
            HighRiskEventCount: assessments.Count(a => a.Level == RiskLevel.High));

        return new RiskForecastResponse(
            area.ToResponse(),
            now,
            summary,
            assessments.Select(a => a.ToResponse()).ToList());
    }
}
