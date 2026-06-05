namespace CarServiceBookingSystem.Application.DTOs.Payments;

public class RefundPaymentResponse
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public decimal RefundedAmount { get; set; }

    public string Currency { get; set; } = string.Empty;

    public string PaymentStatus { get; set; } = string.Empty;

    public string? StripePaymentIntentId { get; set; }

    public string? StripeRefundId { get; set; }

    public DateTime? RefundedAt { get; set; }
}