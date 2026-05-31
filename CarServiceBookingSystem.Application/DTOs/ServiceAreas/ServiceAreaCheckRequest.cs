namespace CarServiceBookingSystem.Application.DTOs.ServiceAreas;

public class ServiceAreaCheckRequest
{
    public int ServiceId { get; set; }

    public string CountryCode { get; set; } = string.Empty;

    public string? City { get; set; }
}