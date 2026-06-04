namespace CarServiceBookingSystem.Application.DTOs.Bookings;

public class BookingCancellationResponse
{
    public int BookingId { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public bool RefundRequired { get; set; }

    public decimal RefundPercentage { get; set; }

    public decimal RefundAmount { get; set; }

    public int? CancellationPolicyRuleId { get; set; }

    public string? PaymentStatus { get; set; }
}