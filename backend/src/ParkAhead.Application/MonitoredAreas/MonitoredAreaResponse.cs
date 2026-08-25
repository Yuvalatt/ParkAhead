namespace ParkAhead.Application.MonitoredAreas;

public record MonitoredAreaResponse(
    Guid Id,
    string Name,
    string Address,
    double Latitude,
    double Longitude,
    double RadiusMeters,
    DateTimeOffset CreatedAt);
