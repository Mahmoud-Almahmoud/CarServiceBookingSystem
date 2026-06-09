using CarServiceBookingSystem.Application.DTOs.Travel;

namespace CarServiceBookingSystem.Application.Interfaces.IGeoLocation;

public interface ITravelEstimateService
{
    Task<TravelEstimateResponse> EstimateAsync(
        decimal originLatitude,
        decimal originLongitude,
        decimal destinationLatitude,
        decimal destinationLongitude,
        CancellationToken cancellationToken = default);
}