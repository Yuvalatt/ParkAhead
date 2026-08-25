using ParkAhead.Infrastructure.EventSources;

namespace ParkAhead.Tests.RiskForecast;

public class MockEventSourceTests
{
    // Roughly Rothschild Blvd, Tel Aviv — same anchor used elsewhere in these tests.
    private const double AreaLatitude = 32.0656;
    private const double AreaLongitude = 34.7742;

    private readonly MockEventSource _source = new();

    [Fact]
    public async Task Excludes_events_outside_the_given_search_radius()
    {
        var now = DateTimeOffset.UtcNow;

        var events = await _source.GetUpcomingEventsAsync(
            AreaLatitude, AreaLongitude, radiusMeters: 5_000, now, now.AddDays(7), CancellationToken.None);

        // mock-evt-6 (Menora Mivtachim Arena) sits ~5.2 km out — just outside a 5 km radius.
        Assert.DoesNotContain(events, e => e.Id == "mock-evt-6");
    }

    [Fact]
    public async Task Includes_events_inside_the_given_search_radius()
    {
        var now = DateTimeOffset.UtcNow;

        var events = await _source.GetUpcomingEventsAsync(
            AreaLatitude, AreaLongitude, radiusMeters: 5_000, now, now.AddDays(7), CancellationToken.None);

        Assert.Contains(events, e => e.Id == "mock-evt-1");
    }

    [Fact]
    public async Task Excludes_events_outside_the_given_time_window()
    {
        var now = DateTimeOffset.UtcNow;

        // A radius generous enough that distance can't be the reason mock-evt-7 is excluded.
        var events = await _source.GetUpcomingEventsAsync(
            AreaLatitude, AreaLongitude, radiusMeters: 20_000, now, now.AddDays(7), CancellationToken.None);

        // mock-evt-7 (Sarona Market) is close by but starts in 10 days — outside a 7-day window.
        Assert.DoesNotContain(events, e => e.Id == "mock-evt-7");
    }

    [Fact]
    public async Task A_wide_enough_radius_and_window_returns_every_event_except_the_deliberately_excluded_ones()
    {
        var now = DateTimeOffset.UtcNow;

        var events = await _source.GetUpcomingEventsAsync(
            AreaLatitude, AreaLongitude, radiusMeters: 20_000, now, now.AddDays(30), CancellationToken.None);

        var ids = events.Select(e => e.Id).ToList();
        Assert.Contains("mock-evt-1", ids);
        Assert.Contains("mock-evt-6", ids); // now within radius
        Assert.Contains("mock-evt-7", ids); // now within window
    }
}
