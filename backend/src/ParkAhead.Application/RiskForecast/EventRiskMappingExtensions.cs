namespace ParkAhead.Application.RiskForecast;

public static class EventRiskMappingExtensions
{
    public static EventRiskResponse ToResponse(this EventRiskAssessment assessment) =>
        new(
            assessment.Event.Id,
            assessment.Event.Title,
            assessment.Event.VenueName,
            assessment.Event.StartTime,
            assessment.Event.Category,
            Math.Round(assessment.DistanceKm, 2),
            assessment.Event.EstimatedAttendance,
            assessment.Score,
            assessment.Level,
            assessment.Reasons);
}
