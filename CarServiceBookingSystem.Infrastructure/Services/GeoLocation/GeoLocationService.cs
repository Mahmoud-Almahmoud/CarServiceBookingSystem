using MaxMind.GeoIP2;
using Microsoft.Extensions.Configuration;

public class GeoLocationService : IGeoLocationService
{
    private readonly string _databasePath;

    public GeoLocationService(IConfiguration configuration)
    {
        _databasePath = configuration["GeoIp:DatabasePath"] ?? string.Empty;
    }

    public Task<GeoLocationResult> GetLocationAsync(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return Task.FromResult(new GeoLocationResult());

        if (ipAddress == "::1" || ipAddress == "127.0.0.1")
        {
            return Task.FromResult(new GeoLocationResult
            {
                Country = "Localhost",
                City = "Localhost"
            });
        }

        if (ipAddress.StartsWith("10.") ||
            ipAddress.StartsWith("192.168.") ||
            ipAddress.StartsWith("172.16.") ||
            ipAddress.StartsWith("172.17.") ||
            ipAddress.StartsWith("172.18.") ||
            ipAddress.StartsWith("172.19.") ||
            ipAddress.StartsWith("172.20.") ||
            ipAddress.StartsWith("172.21.") ||
            ipAddress.StartsWith("172.22.") ||
            ipAddress.StartsWith("172.23.") ||
            ipAddress.StartsWith("172.24.") ||
            ipAddress.StartsWith("172.25.") ||
            ipAddress.StartsWith("172.26.") ||
            ipAddress.StartsWith("172.27.") ||
            ipAddress.StartsWith("172.28.") ||
            ipAddress.StartsWith("172.29.") ||
            ipAddress.StartsWith("172.30.") ||
            ipAddress.StartsWith("172.31."))
        {
            return Task.FromResult(new GeoLocationResult
            {
                Country = "Private Network",
                City = "Private Network"
            });
        }

        if (string.IsNullOrWhiteSpace(_databasePath) || !File.Exists(_databasePath))
            return Task.FromResult(new GeoLocationResult());

        try
        {
            using var reader = new DatabaseReader(_databasePath);

            var city = reader.City(ipAddress);

            return Task.FromResult(new GeoLocationResult
            {
                Country = city.Country.Name,
                City = city.City.Name
            });
        }
        catch
        {
            return Task.FromResult(new GeoLocationResult());
        }
    }
}