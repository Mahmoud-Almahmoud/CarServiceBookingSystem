using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.Payments;

public class PaymentResponse
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public string? UserId { get; set; }

    public string? UserEmail { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = string.Empty;

    public PaymentStatus Status { get; set; }

    public string? StripePaymentIntentId { get; set; }

    public DateTime? PaidAt { get; set; }

    public string? FailureReason { get; set; }

    public BookingStatus BookingStatus { get; set; }

    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public DateTime BookingStartDate { get; set; }

    public DateTime BookingEndDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}