using CarServiceBookingSystem.Application.DTOs.Location;

namespace CarServiceBookingSystem.Application.Interfaces.IGeoLocation;

public interface IReverseGeocodingService
{
    Task<ReverseGeocodeResponse?> ReverseGeocodeAsync(
        decimal latitude,
        decimal longitude,
        CancellationToken cancellationToken = default);
}