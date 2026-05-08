namespace CarServiceBookingSystem.Application.DTOs.Payments;

public class PaymentIntentResponse
{
    public int BookingId { get; set; }
    public string PaymentIntentId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
}