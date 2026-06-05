namespace CarServiceBookingSystem.Application.DTOs.ServicePricing;

public class UpdateServicePriceRuleRequest
{
    public int ServiceId { get; set; }

    public int? CarBrandId { get; set; }

    public int? CarModelId { get; set; }

    public int? CarTrimId { get; set; }

    public int? CarYearId { get; set; }

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }

    public bool IsActive { get; set; }
}