using System.Net;
using System.Net.Http.Json;
using ParkAhead.Application.MonitoredAreas;

namespace ParkAhead.Tests.MonitoredAreas;

/// <summary>
/// Exercises the real HTTP pipeline (model binding + [ApiController] validation), which the
/// controller-level unit tests in <see cref="MonitoredAreasControllerTests"/> bypass entirely.
/// </summary>
public class MonitoredAreasApiTests : IClassFixture<ParkAheadApiFactory>
{
    private readonly HttpClient _client;

    public MonitoredAreasApiTests(ParkAheadApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_with_valid_body_returns_201_with_location_header()
    {
        var request = new CreateMonitoredAreaRequest("Home", 32.08, 34.78, 1500);

        var response = await _client.PostAsJsonAsync("/api/monitored-areas", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var body = await response.Content.ReadFromJsonAsync<MonitoredAreaResponse>();
        Assert.Equal("Home", body!.Name);
    }

    [Theory]
    [InlineData("", 32.08, 34.78, 1500)] // missing name
    [InlineData("Home", 90.01, 34.78, 1500)] // latitude out of range
    [InlineData("Home", 32.08, 34.78, 0)] // non-positive radius
    public async Task Post_with_invalid_body_returns_400(string name, double lat, double lng, double radius)
    {
        var request = new CreateMonitoredAreaRequest(name, lat, lng, radius);

        var response = await _client.PostAsJsonAsync("/api/monitored-areas", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
