using ParkAhead.Domain.Entities;

namespace ParkAhead.Application.MonitoredAreas;

public static class MonitoredAreaMappingExtensions
{
    public static MonitoredAreaResponse ToResponse(this MonitoredArea entity) =>
        new(entity.Id, entity.Name, entity.AreaType, entity.Address, entity.Latitude, entity.Longitude, entity.RadiusMeters, entity.CreatedAt);
}
