namespace CarServiceBookingSystem.Application.DTOs.PromoCodes;

public class PromoCodeQueryRequest
{
    public int? ServiceId { get; set; }

    public int? ServiceBranchId { get; set; }

    public bool? IsActive { get; set; }

    public string? Search { get; set; }

    public string? SortBy { get; set; }
    public bool Desc { get; set; } = true;

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}