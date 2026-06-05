namespace CarServiceBookingSystem.Application.DTOs.ServiceAreas;

public class ServiceAreaCheckResponse
{
    public int ServiceId { get; set; }

    public string CountryCode { get; set; } = string.Empty;

    public string? City { get; set; }

    public bool IsAvailable { get; set; }

    public string Reason { get; set; } = string.Empty;

    public int? MatchedRuleId { get; set; }

    public string? MatchedRuleScope { get; set; }
}