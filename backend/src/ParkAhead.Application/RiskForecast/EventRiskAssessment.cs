using ParkAhead.Domain.Enums;
using ParkAhead.Domain.Events;

namespace ParkAhead.Application.RiskForecast;

/// <summary>Internal engine output for one event; mapped to <see cref="EventRiskResponse"/> at the API boundary.</summary>
public record EventRiskAssessment(
    Event Event,
    double DistanceKm,
    int Score,
    RiskLevel Level,
    IReadOnlyList<string> Reasons);
