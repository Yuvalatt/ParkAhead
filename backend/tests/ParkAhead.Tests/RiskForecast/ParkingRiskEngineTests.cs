using ParkAhead.Application.RiskForecast;
using ParkAhead.Domain.Enums;
using ParkAhead.Domain.Events;

namespace ParkAhead.Tests.RiskForecast;

public class ParkingRiskEngineTests
{
    // Monitored area anchor used across these tests (roughly Rothschild Blvd, Tel Aviv).
    private const double AreaLatitude = 32.0656;
    private const double AreaLongitude = 34.7742;

    private static readonly DateTimeOffset Now = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    private static Event BuildEvent(
        double latitude = AreaLatitude,
        double longitude = AreaLongitude,
        DateTimeOffset? startTime = null,
        EventCategory category = EventCategory.Concert,
        int? estimatedAttendance = 1000) => new()
    {
        Id = "evt-1",
        Title = "Test Event",
        VenueName = "Test Venue",
        Latitude = latitude,
        Longitude = longitude,
        StartTime = startTime ?? Now.AddHours(3),
        Category = category,
        EstimatedAttendance = estimatedAttendance
    };

    [Fact]
    public void Closer_event_scores_higher_than_a_more_distant_event_with_identical_other_factors()
    {
        var close = BuildEvent(latitude: AreaLatitude + 0.003, longitude: AreaLongitude); // ~330 m
        var distant = BuildEvent(latitude: AreaLatitude + 0.03, longitude: AreaLongitude); // ~3.3 km

        var closeAssessment = ParkingRiskEngine.Assess(close, AreaLatitude, AreaLongitude, Now);
        var distantAssessment = ParkingRiskEngine.Assess(distant, AreaLatitude, AreaLongitude, Now);

        Assert.True(closeAssessment.Score > distantAssessment.Score);
    }

    [Fact]
    public void Larger_attendance_scores_higher_than_smaller_attendance_with_identical_other_factors()
    {
        var small = BuildEvent(estimatedAttendance: 200);
        var large = BuildEvent(estimatedAttendance: 12_000);

        var smallAssessment = ParkingRiskEngine.Assess(small, AreaLatitude, AreaLongitude, Now);
        var largeAssessment = ParkingRiskEngine.Assess(large, AreaLatitude, AreaLongitude, Now);

        Assert.True(largeAssessment.Score > smallAssessment.Score);
    }

    [Fact]
    public void Sooner_event_scores_higher_than_a_later_event_with_identical_other_factors()
    {
        var soon = BuildEvent(startTime: Now.AddHours(2));
        var later = BuildEvent(startTime: Now.AddDays(6));

        var soonAssessment = ParkingRiskEngine.Assess(soon, AreaLatitude, AreaLongitude, Now);
        var laterAssessment = ParkingRiskEngine.Assess(later, AreaLatitude, AreaLongitude, Now);

        Assert.True(soonAssessment.Score > laterAssessment.Score);
    }

    [Fact]
    public void Worst_case_event_is_classified_as_high_risk()
    {
        // Very close, huge, starting imminently, high-demand category.
        var worstCase = BuildEvent(
            latitude: AreaLatitude + 0.001,
            longitude: AreaLongitude,
            startTime: Now.AddHours(1),
            category: EventCategory.Concert,
            estimatedAttendance: 20_000);

        var assessment = ParkingRiskEngine.Assess(worstCase, AreaLatitude, AreaLongitude, Now);

        Assert.Equal(RiskLevel.High, assessment.Level);
    }

    [Fact]
    public void Mild_case_event_is_classified_as_low_risk()
    {
        // Far away (outside the event-search radius in practice), small, several days out,
        // low-demand category. The engine itself doesn't enforce the search radius — that's
        // MockEventSource's job — so this only tests the scoring math, not filtering.
        var mildCase = BuildEvent(
            latitude: AreaLatitude + 0.06,
            longitude: AreaLongitude,
            startTime: Now.AddDays(6),
            category: EventCategory.Conference,
            estimatedAttendance: 150);

        var assessment = ParkingRiskEngine.Assess(mildCase, AreaLatitude, AreaLongitude, Now);

        Assert.Equal(RiskLevel.Low, assessment.Level);
    }

    [Fact]
    public void Assessing_the_same_event_twice_produces_an_identical_score()
    {
        var evt = BuildEvent();

        var first = ParkingRiskEngine.Assess(evt, AreaLatitude, AreaLongitude, Now);
        var second = ParkingRiskEngine.Assess(evt, AreaLatitude, AreaLongitude, Now);

        Assert.Equal(first.Score, second.Score);
        Assert.Equal(first.Level, second.Level);
    }

    [Fact]
    public void Score_is_always_within_the_0_to_100_range()
    {
        var extremeHigh = BuildEvent(latitude: AreaLatitude, longitude: AreaLongitude, startTime: Now.AddMinutes(1), estimatedAttendance: 100_000);
        var extremeLow = BuildEvent(latitude: AreaLatitude + 1, longitude: AreaLongitude + 1, startTime: Now.AddDays(7), category: EventCategory.Conference, estimatedAttendance: 1);

        var high = ParkingRiskEngine.Assess(extremeHigh, AreaLatitude, AreaLongitude, Now);
        var low = ParkingRiskEngine.Assess(extremeLow, AreaLatitude, AreaLongitude, Now);

        Assert.InRange(high.Score, 0, 100);
        Assert.InRange(low.Score, 0, 100);
    }

    [Fact]
    public void Reasons_explain_distance_attendance_and_timing()
    {
        var evt = BuildEvent(
            latitude: AreaLatitude + 0.003,
            estimatedAttendance: 9000,
            startTime: Now.AddHours(3));

        var assessment = ParkingRiskEngine.Assess(evt, AreaLatitude, AreaLongitude, Now);

        Assert.Contains(assessment.Reasons, r => r.Contains("from your monitored area"));
        Assert.Contains(assessment.Reasons, r => r.Contains("attendees"));
        Assert.Contains(assessment.Reasons, r => r.Contains("Starts in"));
    }

    [Fact]
    public void Reasons_include_a_category_note_for_high_demand_categories_but_not_conferences()
    {
        var concert = BuildEvent(category: EventCategory.Concert);
        var conference = BuildEvent(category: EventCategory.Conference);

        var concertAssessment = ParkingRiskEngine.Assess(concert, AreaLatitude, AreaLongitude, Now);
        var conferenceAssessment = ParkingRiskEngine.Assess(conference, AreaLatitude, AreaLongitude, Now);

        Assert.Contains(concertAssessment.Reasons, r => r.Contains("concentrated parking demand"));
        Assert.DoesNotContain(conferenceAssessment.Reasons, r => r.Contains("concentrated parking demand"));
    }

    [Fact]
    public void Unknown_attendance_is_explained_explicitly()
    {
        var evt = BuildEvent(estimatedAttendance: null);

        var assessment = ParkingRiskEngine.Assess(evt, AreaLatitude, AreaLongitude, Now);

        Assert.Contains(assessment.Reasons, r => r == "Attendance size unknown");
    }
}
