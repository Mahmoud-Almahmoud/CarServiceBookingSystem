namespace CarServiceBookingSystem.Application.DTOs.Location;

public class ReverseGeocodeResponse
{
    public string CountryCode { get; set; } = string.Empty;

    public string CountryName { get; set; } = string.Empty;

    public string? City { get; set; }

    public string? FormattedAddress { get; set; }
}