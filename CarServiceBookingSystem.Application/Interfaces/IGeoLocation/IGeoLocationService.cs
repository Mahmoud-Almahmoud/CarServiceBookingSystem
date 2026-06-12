public interface IGeoLocationService
{
    Task<GeoLocationResult> GetLocationAsync(string? ipAddress);
}