namespace CarServiceBookingSystem.Application.DTOs.PromoCodes;

public class PromoCodeValidationRequest
{
    public string PromoCode { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public int ServiceId { get; set; }

    public int? ServiceBranchId { get; set; }

    public decimal SubtotalPrice { get; set; }
}