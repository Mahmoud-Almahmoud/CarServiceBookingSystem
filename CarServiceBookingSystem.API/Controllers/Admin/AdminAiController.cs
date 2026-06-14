using Asp.Versioning;
using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using CarServiceBookingSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/ai")]
[Authorize(Roles = Roles.Admin)]
public sealed class AdminAiController : ControllerBase
{
    private readonly IAiAnalyticsService _analyticsService;

    public AdminAiController(IAiAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("analytics/overview")]
    [ProducesResponseType(typeof(ApiResponse<AiAnalyticsOverviewDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AiAnalyticsOverviewDto>>> GetOverview(
        CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetOverviewAsync(cancellationToken);

        return Ok(ApiResponse<AiAnalyticsOverviewDto>.Ok(result));
    }

    [HttpGet("analytics/top-services")]
    [ProducesResponseType(typeof(ApiResponse<List<AiTopRecommendedServiceDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AiTopRecommendedServiceDto>>>> GetTopServices(
        [FromQuery] int take = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _analyticsService.GetTopRecommendedServicesAsync(
            take,
            cancellationToken);

        return Ok(ApiResponse<List<AiTopRecommendedServiceDto>>.Ok(result));
    }

    [HttpGet("analytics/daily-usage")]
    [ProducesResponseType(typeof(ApiResponse<List<AiDailyUsageDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AiDailyUsageDto>>>> GetDailyUsage(
        [FromQuery] int days = 14,
        CancellationToken cancellationToken = default)
    {
        var result = await _analyticsService.GetDailyUsageAsync(
            days,
            cancellationToken);

        return Ok(ApiResponse<List<AiDailyUsageDto>>.Ok(result));
    }
}