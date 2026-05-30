namespace CarServiceBookingSystem.Application.Options;

public class BookingQuoteOptions
{
    public decimal DefaultBranchLatitude { get; set; }

    public decimal DefaultBranchLongitude { get; set; }

    public double MockAverageSpeedKmPerHour { get; set; } = 40;

    public int MinimumTravelTimeMinutes { get; set; } = 10;

    public decimal BaseTravelFee { get; set; } = 20;

    public decimal TravelFeePerKm { get; set; } = 2;

    public decimal MaxTravelFee { get; set; } = 150;
}