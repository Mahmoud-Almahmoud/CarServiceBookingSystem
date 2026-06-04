using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.CancellationPolicyRules;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface ICancellationPolicyRuleService
{
    Task<ApiResponse<PagedResponse<CancellationPolicyRuleResponse>>> GetAllAsync(
        CancellationPolicyRuleQueryRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<CancellationPolicyRuleResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<CancellationPolicyRuleResponse>> CreateAsync(
        CreateCancellationPolicyRuleRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<CancellationPolicyRuleResponse>> UpdateAsync(
        int id,
        UpdateCancellationPolicyRuleRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<CancellationPolicyQuoteResponse> CalculateCancellationPolicyAsync(
        int serviceId,
        int? serviceBranchId,
        DateTime bookingStartDate,
        decimal paidAmount,
        bool hasSucceededPayment,
        CancellationToken cancellationToken = default);
}