using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ParkAhead.Application.MonitoredAreas;
using ParkAhead.Domain.Enums;

namespace ParkAhead.Tests.MonitoredAreas;

/// <summary>
/// Exercises the real HTTP pipeline (model binding + [ApiController] validation), which the
/// controller-level unit tests in <see cref="MonitoredAreasControllerTests"/> bypass entirely.
/// </summary>
public class MonitoredAreasApiTests : IClassFixture<ParkAheadApiFactory>
{
    private const string ValidAddress = "1 Rothschild Blvd, Tel Aviv-Yafo, Israel";

    // Plain HttpClient.ReadFromJsonAsync uses default JsonSerializerOptions, which doesn't know
    // AreaType serializes as a string on the wire (that converter is registered on the server's
    // MVC pipeline only) — tests need their own copy to deserialize the response correctly.
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly HttpClient _client;

    public MonitoredAreasApiTests(ParkAheadApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_with_valid_body_returns_201_with_location_header()
    {
        var request = new CreateMonitoredAreaRequest("Home", AreaType.Home, ValidAddress, 32.08, 34.78, 1500);

        var response = await _client.PostAsJsonAsync("/api/monitored-areas", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var body = await response.Content.ReadFromJsonAsync<MonitoredAreaResponse>(JsonOptions);
        Assert.Equal("Home", body!.Name);
        Assert.Equal(AreaType.Home, body.AreaType);
        Assert.Equal(ValidAddress, body.Address);
    }

    [Fact]
    public async Task AreaType_serializes_as_its_string_name_not_a_number()
    {
        var request = new CreateMonitoredAreaRequest("Office", AreaType.Work, ValidAddress, 32.08, 34.78, 1500);

        var response = await _client.PostAsJsonAsync("/api/monitored-areas", request);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"areaType\":\"Work\"", json);
    }

    [Theory]
    [InlineData("", ValidAddress, 32.08, 34.78, 1500)] // missing name
    [InlineData("Home", "", 32.08, 34.78, 1500)] // missing address
    [InlineData("Home", ValidAddress, 90.01, 34.78, 1500)] // latitude out of range
    [InlineData("Home", ValidAddress, 32.08, 34.78, 0)] // non-positive radius
    public async Task Post_with_invalid_body_returns_400(string name, string address, double lat, double lng, double radius)
    {
        var request = new CreateMonitoredAreaRequest(name, AreaType.Home, address, lat, lng, radius);

        var response = await _client.PostAsJsonAsync("/api/monitored-areas", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_with_an_out_of_range_area_type_returns_400()
    {
        // AreaType can't express an invalid value through the strongly-typed record constructor,
        // so this posts raw JSON to exercise the [EnumDataType] validation directly.
        var json = $$"""{"name":"Home","areaType":99,"address":"{{ValidAddress}}","latitude":32.08,"longitude":34.78,"radiusMeters":1500}""";
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/monitored-areas", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
