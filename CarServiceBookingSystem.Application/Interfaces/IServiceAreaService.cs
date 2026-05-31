using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.ServiceAreas;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface IServiceAreaService
{
    Task<ApiResponse<PagedResponse<ServiceAreaRuleResponse>>> GetRulesAsync(
        ServiceAreaRuleFilterRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ServiceAreaRuleResponse>> GetRuleByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ServiceAreaRuleResponse>> CreateRuleAsync(
        CreateServiceAreaRuleRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ServiceAreaRuleResponse>> UpdateRuleAsync(
        int id,
        UpdateServiceAreaRuleRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<string>> DeleteRuleAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ServiceAreaCheckResponse>> CheckAvailabilityAsync(
        ServiceAreaCheckRequest request,
        CancellationToken cancellationToken = default);
}