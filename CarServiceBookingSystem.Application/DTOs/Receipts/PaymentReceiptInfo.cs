namespace CarServiceBookingSystem.Application.DTOs.Receipts;

public class PaymentReceiptInfo
{
    public int PaymentId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime? PaidAt { get; set; }

    public string? StripePaymentIntentId { get; set; }

    public string? StripeRefundId { get; set; }

    public decimal? RefundedAmount { get; set; }

    public DateTime? RefundedAt { get; set; }
}