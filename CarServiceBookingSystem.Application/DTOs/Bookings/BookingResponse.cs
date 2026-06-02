using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.Bookings;

public class BookingResponse
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int CarId { get; set; }
    public string PlateNumber { get; set; } = string.Empty;

    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;

    public int? ServiceBranchId { get; set; }
    public string? ServiceBranchName { get; set; }

    public int? TechnicianId { get; set; }
    public string? TechnicianName { get; set; }

    
    public decimal ServicePrice { get; set; }
    public decimal TravelFee { get; set; }
    public decimal TotalPrice { get; set; }

    public decimal? CustomerLatitude { get; set; }
    public decimal? CustomerLongitude { get; set; }
    public string? CustomerCountryCode { get; set; }
    public string? CustomerCity { get; set; }
    public string? CustomerFormattedAddress { get; set; }

    public double? DistanceKm { get; set; }
    public int? EstimatedTravelTimeMinutes { get; set; }

    public int? ServicePriceRuleId { get; set; }
    public int? ServiceAreaRuleId { get; set; }

    public ServiceLocationType LocationType { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public BookingStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}