using ParkAhead.Domain.Enums;

namespace ParkAhead.Application.RiskForecast;

public record EventRiskResponse(
    string EventId,
    string Title,
    string? VenueName,
    DateTimeOffset StartTime,
    EventCategory Category,
    double DistanceKm,
    int? EstimatedAttendance,
    int RiskScore,
    RiskLevel RiskLevel,
    IReadOnlyList<string> Reasons);
