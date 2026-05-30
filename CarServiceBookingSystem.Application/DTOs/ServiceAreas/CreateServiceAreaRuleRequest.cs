namespace CarServiceBookingSystem.Application.DTOs.ServiceAreas;

public class CreateServiceAreaRuleRequest
{
    public int? ServiceId { get; set; }

    public string CountryCode { get; set; } = string.Empty;

    public string? City { get; set; }

    public bool IsAllowed { get; set; }

    public int Priority { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}