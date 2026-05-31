namespace CarServiceBookingSystem.Application.Options;

public class OpenRouteServiceOptions
{
    public string ApiKey { get; set; } = string.Empty;

    public string BaseUrl { get; set; } = "https://api.openrouteservice.org/";
}