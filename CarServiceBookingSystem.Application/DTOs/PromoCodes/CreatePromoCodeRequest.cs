namespace CarServiceBookingSystem.Application.DTOs.PromoCodes;

public class CreatePromoCodeRequest
{
    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string DiscountType { get; set; } = string.Empty;

    public decimal DiscountValue { get; set; }

    public decimal? MaxDiscountAmount { get; set; }

    public decimal? MinimumSubtotalAmount { get; set; }

    public int? ServiceId { get; set; }

    public int? ServiceBranchId { get; set; }

    public DateTime? StartsAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public int? MaxTotalRedemptions { get; set; }

    public int? MaxRedemptionsPerUser { get; set; }

    public bool IsActive { get; set; } = true;
}