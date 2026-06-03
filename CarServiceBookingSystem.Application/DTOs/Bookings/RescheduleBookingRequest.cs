namespace CarServiceBookingSystem.Application.DTOs.Bookings;

public class RescheduleBookingRequest
{
    public DateTime StartDate { get; set; }

    public int? ServiceBranchId { get; set; }

    public decimal? CustomerLatitude { get; set; }
    public decimal? CustomerLongitude { get; set; }

    public string? CustomerCountryCode { get; set; }
    public string? CustomerCity { get; set; }
}