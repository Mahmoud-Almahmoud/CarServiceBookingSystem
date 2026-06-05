using CarServiceBookingSystem.Application.DTOs.Travel;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Options;
using Microsoft.Extensions.Options;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class MockTravelEstimateService : ITravelEstimateService
{
    private readonly BookingQuoteOptions _options;

    public MockTravelEstimateService(IOptions<BookingQuoteOptions> options)
    {
        _options = options.Value;
    }

    public Task<TravelEstimateResponse> EstimateAsync(
        decimal originLatitude,
        decimal originLongitude,
        decimal destinationLatitude,
        decimal destinationLongitude,
        CancellationToken cancellationToken = default)
    {
        var distanceKm = CalculateDistanceKm(
            (double)originLatitude,
            (double)originLongitude,
            (double)destinationLatitude,
            (double)destinationLongitude);

        var speed = _options.MockAverageSpeedKmPerHour <= 0
            ? 40
            : _options.MockAverageSpeedKmPerHour;

        var estimatedMinutes = (int)Math.Ceiling(distanceKm / speed * 60);

        estimatedMinutes = Math.Max(
            estimatedMinutes,
            _options.MinimumTravelTimeMinutes);

        var response = new TravelEstimateResponse
        {
            DistanceKm = Math.Round(distanceKm, 2),
            EstimatedTravelTimeMinutes = estimatedMinutes
        };

        return Task.FromResult(response);
    }

    private static double CalculateDistanceKm(
        double latitude1,
        double longitude1,
        double latitude2,
        double longitude2)
    {
        const double earthRadiusKm = 6371;

        var dLat = DegreesToRadians(latitude2 - latitude1);
        var dLon = DegreesToRadians(longitude2 - longitude1);

        var lat1 = DegreesToRadians(latitude1);
        var lat2 = DegreesToRadians(latitude2);

        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2) *
            Math.Cos(lat1) * Math.Cos(lat2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}