using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ParkAhead.Application.MonitoredAreas;
using ParkAhead.Application.RiskForecast;
using ParkAhead.Domain.Enums;
using ParkAhead.Tests.MonitoredAreas;

namespace ParkAhead.Tests.RiskForecast;

/// <summary>Exercises GET /api/monitored-areas/{id}/risk-forecast through the real HTTP pipeline.</summary>
public class RiskForecastApiTests : IClassFixture<ParkAheadApiFactory>
{
    // Server-side enums (AreaType, EventCategory, RiskLevel) serialize as strings — see
    // Program.cs — but a plain HttpClient's default deserializer doesn't know that.
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly HttpClient _client;

    public RiskForecastApiTests(ParkAheadApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRiskForecast_returns_404_for_a_missing_monitored_area()
    {
        var response = await _client.GetAsync($"/api/monitored-areas/{Guid.NewGuid()}/risk-forecast");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetRiskForecast_returns_scored_nearby_events_for_an_existing_area()
    {
        // Central Tel Aviv, close to the mock event set.
        var createRequest = new CreateMonitoredAreaRequest(
            "Home", AreaType.Home, "1 Rothschild Blvd, Tel Aviv-Yafo, Israel", 32.0656, 34.7742, 1500);
        var createResponse = await _client.PostAsJsonAsync("/api/monitored-areas", createRequest, JsonOptions);
        var area = await createResponse.Content.ReadFromJsonAsync<MonitoredAreaResponse>(JsonOptions);

        var response = await _client.GetAsync($"/api/monitored-areas/{area!.Id}/risk-forecast");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var forecast = await response.Content.ReadFromJsonAsync<RiskForecastResponse>(JsonOptions);
        Assert.NotNull(forecast);
        Assert.Equal(area.Id, forecast!.MonitoredArea.Id);
        Assert.NotEmpty(forecast.Events);
        Assert.Equal(forecast.Events.Count, forecast.Summary.UpcomingEventCount);
        Assert.All(forecast.Events, e => Assert.InRange(e.RiskScore, 0, 100));

        // Results should be sorted by descending risk score.
        var scores = forecast.Events.Select(e => e.RiskScore).ToList();
        Assert.Equal(scores.OrderByDescending(s => s), scores);

        // Events deliberately outside the default radius/window (see MockEventSource) never
        // reach the API response.
        Assert.DoesNotContain(forecast.Events, e => e.EventId is "mock-evt-6" or "mock-evt-7");
    }
}
