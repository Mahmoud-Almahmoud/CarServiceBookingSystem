namespace CarServiceBookingSystem.Application.DTOs.ServiceAreas;

public class ServiceAreaRuleResponse
{
    public int Id { get; set; }

    public int? ServiceId { get; set; }

    public string? ServiceName { get; set; }

    public string CountryCode { get; set; } = string.Empty;

    public string? City { get; set; }

    public bool IsAllowed { get; set; }

    public int Priority { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}