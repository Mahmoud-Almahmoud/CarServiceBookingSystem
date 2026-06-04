namespace CarServiceBookingSystem.Domain.Entities;

public class PromoCodeRedemption : BaseEntity
{
    public int PromoCodeId { get; set; }
    public PromoCode PromoCode { get; set; } = null!;

    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;

    public decimal DiscountAmount { get; set; }

}