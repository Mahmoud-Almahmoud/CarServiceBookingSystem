using CarServiceBookingSystem.Application.DTOs.Ai;

namespace CarServiceBookingSystem.Application.Interfaces.IAi;

public interface IAiAnalyticsQuery
{
    Task<AiAnalyticsOverviewDto> GetOverviewAsync(
        CancellationToken cancellationToken = default);

    Task<List<AiTopRecommendedServiceDto>> GetTopRecommendedServicesAsync(
        int take = 10,
        CancellationToken cancellationToken = default);

    Task<List<AiDailyUsageDto>> GetDailyUsageAsync(
        int days = 14,
        CancellationToken cancellationToken = default);
}