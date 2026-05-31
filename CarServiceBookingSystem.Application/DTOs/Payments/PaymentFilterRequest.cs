using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.Payments;

public class PaymentFilterRequest : PagedRequest
{
    public int? BookingId { get; set; }

    public PaymentStatus? Status { get; set; }

    public string? Currency { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public decimal? MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }
}