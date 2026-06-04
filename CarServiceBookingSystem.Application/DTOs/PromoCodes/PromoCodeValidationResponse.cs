namespace CarServiceBookingSystem.Application.DTOs.PromoCodes;

public class PromoCodeValidationResponse
{
    public bool IsValid { get; set; }

    public int? PromoCodeId { get; set; }

    public string? PromoCode { get; set; }

    public decimal DiscountAmount { get; set; }

    public string? Message { get; set; }
}