using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.Bookings;

public class BookingQuoteResponse
{
    public int CarId { get; set; }

    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;

    public int? ServiceBranchId { get; set; }
    public string? ServiceBranchName { get; set; }

    public double? BranchStraightLineDistanceKm { get; set; }

    public ServiceLocationType LocationType { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int DurationMinutes { get; set; }

    public decimal ServicePrice { get; set; }
    public decimal TravelFee { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal SubtotalPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public int? PromoCodeId { get; set; }
    public string? PromoCode { get; set; }
    public string? PromoCodeMessage { get; set; }

    public bool IsAvailable { get; set; }
    public string? UnavailableReason { get; set; }

    public decimal? CustomerLatitude { get; set; }
    public decimal? CustomerLongitude { get; set; }
    public string? CustomerFormattedAddress { get; set; }

    public double? DistanceKm { get; set; }
    public int? EstimatedTravelTimeMinutes { get; set; }

    public bool UsedCustomPriceRule { get; set; }
    public int? ServicePriceRuleId { get; set; }
    public string PricingSource { get; set; } = string.Empty;

    public string? CustomerCountryCode { get; set; }
    public string? CustomerCity { get; set; }

    public int? MatchedServiceAreaRuleId { get; set; }
    public string? MatchedServiceAreaRuleScope { get; set; }
}