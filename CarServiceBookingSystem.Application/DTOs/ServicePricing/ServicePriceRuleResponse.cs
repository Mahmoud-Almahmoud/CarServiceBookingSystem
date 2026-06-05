namespace CarServiceBookingSystem.Application.DTOs.ServicePricing;

public class ServicePriceRuleResponse
{
    public int Id { get; set; }

    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public int? CarBrandId { get; set; }

    public string? CarBrandName { get; set; }

    public int? CarModelId { get; set; }

    public string? CarModelName { get; set; }

    public int? CarTrimId { get; set; }

    public string? CarTrimName { get; set; }

    public int? CarYearId { get; set; }
    public int? CarYearValue { get; set; }

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}