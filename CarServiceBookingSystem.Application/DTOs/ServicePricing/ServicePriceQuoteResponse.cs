namespace CarServiceBookingSystem.Application.DTOs.ServicePricing;

public class ServicePriceQuoteResponse
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public int CarId { get; set; }
    public int CarBrandId { get; set; }
    public string CarBrandName { get; set; } = string.Empty;

    public int CarModelId { get; set; }
    public string CarModelName { get; set; } = string.Empty;

    public int CarYearId { get; set; }
    public int CarYearValue { get; set; }

    public int CarTrimId { get; set; }
    public string CarTrimName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }

    public bool UsedCustomPriceRule { get; set; }

    public int? ServicePriceRuleId { get; set; }

    public string PricingSource { get; set; } = string.Empty;
}