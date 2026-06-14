using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;

namespace CarServiceBookingSystem.Application.Services.Ai;

public sealed class AiAnalyticsService : IAiAnalyticsService
{
    private readonly IAiAnalyticsQuery _analyticsQuery;

    public AiAnalyticsService(IAiAnalyticsQuery analyticsQuery)
    {
        _analyticsQuery = analyticsQuery;
    }

    public Task<AiAnalyticsOverviewDto> GetOverviewAsync(
        CancellationToken cancellationToken = default)
    {
        return _analyticsQuery.GetOverviewAsync(cancellationToken);
    }

    public Task<List<AiTopRecommendedServiceDto>> GetTopRecommendedServicesAsync(
        int take = 10,
        CancellationToken cancellationToken = default)
    {
        take = Math.Clamp(take, 1, 50);

        return _analyticsQuery.GetTopRecommendedServicesAsync(
            take,
            cancellationToken);
    }

    public Task<List<AiDailyUsageDto>> GetDailyUsageAsync(
        int days = 14,
        CancellationToken cancellationToken = default)
    {
        days = Math.Clamp(days, 1, 90);

        return _analyticsQuery.GetDailyUsageAsync(
            days,
            cancellationToken);
    }
}