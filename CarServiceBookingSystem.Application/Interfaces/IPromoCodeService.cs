using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.PromoCodes;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface IPromoCodeService
{
    Task<ApiResponse<PagedResponse<PromoCodeResponse>>> GetAllAsync(
        PromoCodeQueryRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PromoCodeResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PromoCodeResponse>> CreateAsync(
        CreatePromoCodeRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PromoCodeResponse>> UpdateAsync(
        int id,
        UpdatePromoCodeRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<PromoCodeValidationResponse> ValidateAsync(
        PromoCodeValidationRequest request,
        CancellationToken cancellationToken = default);

    Task RedeemAsync(
        int promoCodeId,
        int bookingId,
        string userId,
        decimal discountAmount,
        CancellationToken cancellationToken = default);
}