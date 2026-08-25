namespace ParkAhead.Application.MonitoredAreas;

public record MonitoredAreaResponse(
    Guid Id,
    string Name,
    double Latitude,
    double Longitude,
    double RadiusMeters,
    DateTimeOffset CreatedAt);
