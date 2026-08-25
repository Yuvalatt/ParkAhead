using ParkAhead.Domain.Enums;

namespace ParkAhead.Application.MonitoredAreas;

public record MonitoredAreaResponse(
    Guid Id,
    string Name,
    AreaType AreaType,
    string Address,
    double Latitude,
    double Longitude,
    double RadiusMeters,
    DateTimeOffset CreatedAt);
