using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Domain.Entities;

public class PromoCode : BaseIdEntity
{
    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DiscountType DiscountType { get; set; }

    public decimal DiscountValue { get; set; }

    public decimal? MaxDiscountAmount { get; set; }

    public decimal? MinimumSubtotalAmount { get; set; }

    public int? ServiceId { get; set; }
    public Service? Service { get; set; }

    public int? ServiceBranchId { get; set; }
    public ServiceBranch? ServiceBranch { get; set; }

    public DateTime? StartsAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public int? MaxTotalRedemptions { get; set; }

    public int? MaxRedemptionsPerUser { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<PromoCodeRedemption> Redemptions { get; set; } = new List<PromoCodeRedemption>();

}