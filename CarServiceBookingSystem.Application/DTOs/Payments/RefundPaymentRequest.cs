namespace CarServiceBookingSystem.Application.DTOs.Payments;

public class RefundPaymentRequest
{
    public decimal? Amount { get; set; }
    public string? Reason { get; set; }
}