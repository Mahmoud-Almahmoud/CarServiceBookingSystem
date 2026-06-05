namespace CarServiceBookingSystem.Application.DTOs.Receipts;

public class BookingPriceReceiptInfo
{
    public decimal ServicePrice { get; set; }

    public decimal TravelFee { get; set; }

    public decimal SubtotalPrice { get; set; }

    public decimal DiscountAmount { get; set; }

    public string? PromoCode { get; set; }

    public decimal TotalPrice { get; set; }

    public string Currency { get; set; } = "aed";
}