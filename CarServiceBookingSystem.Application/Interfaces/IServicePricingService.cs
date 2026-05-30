using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.ServicePricing;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface IServicePricingService
{
    Task<ApiResponse<PagedResponse<ServicePriceRuleResponse>>> GetRulesAsync(
        ServicePriceRuleFilterRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ServicePriceRuleResponse>> GetRuleByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ServicePriceRuleResponse>> CreateRuleAsync(
        CreateServicePriceRuleRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ServicePriceRuleResponse>> UpdateRuleAsync(
        int id,
        UpdateServicePriceRuleRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<string>> DeleteRuleAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ServicePriceQuoteResponse>> GetPriceQuoteAsync(
        ServicePriceQuoteRequest request,
        string userId,
        CancellationToken cancellationToken = default);
}