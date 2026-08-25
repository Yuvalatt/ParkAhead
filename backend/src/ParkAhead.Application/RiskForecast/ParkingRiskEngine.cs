using ParkAhead.Domain.Enums;
using ParkAhead.Domain.Events;

namespace ParkAhead.Application.RiskForecast;

/// <summary>
/// Deterministic, explainable parking-risk scoring for a single event relative to a monitored area.
/// No randomness and no ML — a weighted sum of four independently-tunable 0–1 factors, scaled to 0–100.
/// Pure and stateless by design so it's trivially unit-testable without mocking anything.
/// </summary>
public static class ParkingRiskEngine
{
    // Factor weights — must sum to 1.0. Distance dominates because proximity is the single
    // strongest predictor of whether an event's traffic actually spills into a nearby monitored
    // area; category is the smallest nudge, per the product spec.
    private const double DistanceWeight = 0.40;
    private const double AttendanceWeight = 0.30;
    private const double TimeWeight = 0.20;
    private const double CategoryWeight = 0.10;

    // Risk-level thresholds on the final 0–100 score. Centralized here so tuning the cutoffs
    // never requires touching the scoring math itself.
    private const int MediumThreshold = 35;
    private const int HighThreshold = 70;

    public static EventRiskAssessment Assess(
        Event @event,
        double monitoredAreaLatitude,
        double monitoredAreaLongitude,
        DateTimeOffset now)
    {
        var distanceKm = DistanceCalculator.CalculateKilometers(
            monitoredAreaLatitude, monitoredAreaLongitude, @event.Latitude, @event.Longitude);

        var rawScore = DistanceWeight * ScoreDistance(distanceKm)
                      + AttendanceWeight * ScoreAttendance(@event.EstimatedAttendance)
                      + TimeWeight * ScoreTime(@event.StartTime, now)
                      + CategoryWeight * ScoreCategory(@event.Category);

        var score = (int)Math.Round(rawScore * 100, MidpointRounding.AwayFromZero);
        var level = ToRiskLevel(score);
        var reasons = BuildReasons(@event, distanceKm, now);

        return new EventRiskAssessment(@event, distanceKm, score, level, reasons);
    }

    // <=0.5km very strong, <=1km strong, <=2km moderate, <=5km decreasing, beyond: negligible.
    private static double ScoreDistance(double distanceKm) => distanceKm switch
    {
        <= 0.5 => 1.0,
        <= 1.0 => 0.75,
        <= 2.0 => 0.5,
        <= 5.0 => 0.25,
        _ => 0.0
    };

    // Unknown attendance defaults to a modest middle value rather than 0 — an event we simply
    // don't have a headcount for isn't necessarily small.
    private static double ScoreAttendance(int? attendance) => attendance switch
    {
        null => 0.3,
        < 500 => 0.15,
        < 2_000 => 0.4,
        < 8_000 => 0.7,
        _ => 1.0
    };

    private static double ScoreTime(DateTimeOffset startTime, DateTimeOffset now)
    {
        var hoursUntilStart = (startTime - now).TotalHours;
        return hoursUntilStart switch
        {
            <= 6 => 1.0,
            <= 24 => 0.8,
            <= 72 => 0.5,
            <= 168 => 0.25, // 7 days — matches the forecast window
            _ => 0.0
        };
    }

    // Concerts/festivals/sports concentrate arrivals and departures around a single venue and
    // time; conferences typically spread parking demand across the day and often have dedicated
    // venue parking, so they get a smaller bump.
    private static double ScoreCategory(EventCategory category) => category switch
    {
        EventCategory.Concert => 1.0,
        EventCategory.Festival => 1.0,
        EventCategory.Sports => 0.9,
        EventCategory.Other => 0.5,
        EventCategory.Conference => 0.4,
        _ => 0.5
    };

    private static RiskLevel ToRiskLevel(int score) => score switch
    {
        _ when score >= HighThreshold => RiskLevel.High,
        _ when score >= MediumThreshold => RiskLevel.Medium,
        _ => RiskLevel.Low
    };

    private static List<string> BuildReasons(Event @event, double distanceKm, DateTimeOffset now)
    {
        var reasons = new List<string>
        {
            DescribeDistance(distanceKm),
            DescribeAttendance(@event.EstimatedAttendance),
            DescribeTiming(@event.StartTime, now)
        };

        if (@event.Category is EventCategory.Concert or EventCategory.Festival or EventCategory.Sports)
        {
            reasons.Add($"{@event.Category} events typically create concentrated parking demand.");
        }

        return reasons;
    }

    private static string DescribeDistance(double distanceKm) =>
        distanceKm < 1
            ? $"{Math.Round(distanceKm * 1000 / 50.0) * 50:0} m from your monitored area"
            : $"{distanceKm:0.#} km from your monitored area";

    private static string DescribeAttendance(int? attendance) => attendance switch
    {
        null => "Attendance size unknown",
        < 500 => $"Small event with ~{attendance} attendees",
        < 2_000 => $"Medium-sized event with ~{attendance:N0} attendees",
        < 8_000 => $"Large event with ~{attendance:N0} attendees",
        _ => $"Very large event with ~{attendance:N0} attendees"
    };

    private static string DescribeTiming(DateTimeOffset startTime, DateTimeOffset now)
    {
        var span = startTime - now;

        if (span.TotalHours < 1) return "Starting soon";
        if (span.TotalHours < 24) return FormatHours((int)Math.Round(span.TotalHours));

        var days = (int)Math.Round(span.TotalDays);
        return days <= 1 ? "Starts tomorrow" : $"Starts in {days} days";
    }

    private static string FormatHours(int hours) => hours == 1 ? "Starts in 1 hour" : $"Starts in {hours} hours";
}
