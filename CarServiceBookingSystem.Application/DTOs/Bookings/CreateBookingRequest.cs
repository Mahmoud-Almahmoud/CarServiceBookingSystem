using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.Bookings;

public class CreateBookingRequest
{
    public int CarId { get; set; }
    public int ServiceId { get; set; }
    public int? ServiceBranchId { get; set; }
    public ServiceLocationType LocationType { get; set; }
    public DateTime StartDate { get; set; }
    public decimal? CustomerLatitude { get; set; }

    public decimal? CustomerLongitude { get; set; }

    public string? CustomerCountryCode { get; set; }

    public string? CustomerCity { get; set; }
}