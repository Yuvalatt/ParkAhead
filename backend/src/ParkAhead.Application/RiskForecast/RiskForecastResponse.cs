using ParkAhead.Application.MonitoredAreas;

namespace ParkAhead.Application.RiskForecast;

public record RiskForecastResponse(
    MonitoredAreaResponse MonitoredArea,
    DateTimeOffset GeneratedAt,
    RiskForecastSummary Summary,
    IReadOnlyList<EventRiskResponse> Events);
