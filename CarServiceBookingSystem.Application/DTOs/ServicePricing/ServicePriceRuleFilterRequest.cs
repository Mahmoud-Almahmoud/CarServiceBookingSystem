namespace CarServiceBookingSystem.Application.DTOs.ServicePricing;

public class ServicePriceRuleFilterRequest
{
    public int? ServiceId { get; set; }

    public int? CarBrandId { get; set; }

    public int? CarModelId { get; set; }
    public int? CarYearId { get; set; }

    public int? CarTrimId { get; set; }

    public bool? IsActive { get; set; }

    public string? Search { get; set; }

    public string? SortBy { get; set; }

    public bool Desc { get; set; } = true;

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}