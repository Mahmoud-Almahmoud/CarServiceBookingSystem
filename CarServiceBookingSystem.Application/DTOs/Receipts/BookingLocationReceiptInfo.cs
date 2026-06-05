namespace CarServiceBookingSystem.Application.DTOs.Receipts;

public class BookingLocationReceiptInfo
{
    public string LocationType { get; set; } = string.Empty;

    public decimal? CustomerLatitude { get; set; }

    public decimal? CustomerLongitude { get; set; }

    public string? CustomerCountryCode { get; set; }

    public string? CustomerCity { get; set; }

    public string? CustomerFormattedAddress { get; set; }

    public decimal? DistanceKm { get; set; }

    public int? EstimatedTravelTimeMinutes { get; set; }
}