namespace CarServiceBookingSystem.Application.Options;

public class GoogleMapsOptions
{
    public string ApiKey { get; set; } = string.Empty;

    public string GeocodingBaseUrl { get; set; } = "https://maps.googleapis.com/maps/api/geocode/";

    public string RoutesBaseUrl { get; set; } = "https://routes.googleapis.com/";
}