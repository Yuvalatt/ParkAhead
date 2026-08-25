using System.ComponentModel.DataAnnotations;

namespace ParkAhead.Application.MonitoredAreas;

// Validation attributes must sit directly on the primary-constructor parameters (not [property: ...]) —
// ASP.NET Core's model validator throws at request time if record validation metadata lives on the property.
public record CreateMonitoredAreaRequest(
    [Required, MaxLength(200)] string Name,
    // The formatted address as resolved by the frontend's geocoding source (Google Places).
    // Stored for display; latitude/longitude are still validated below regardless of origin.
    [Required, MaxLength(500)] string Address,
    // Explicit double literals matter here: Range(-90, 90) would bind to the int overload,
    // which converts the validated value to int before comparing and silently truncates it.
    [Range(-90.0, 90.0)] double Latitude,
    [Range(-180.0, 180.0)] double Longitude,
    // 20km is a generous upper bound for a neighborhood-scale "area I care about".
    [Range(1.0, 20_000.0)] double RadiusMeters);
