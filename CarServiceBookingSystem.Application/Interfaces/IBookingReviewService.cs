using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Reviews;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface IBookingReviewService
{
    Task<ApiResponse<BookingReviewResponse>> CreateMyReviewAsync(
        int bookingId,
        CreateBookingReviewRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<BookingReviewResponse>> UpdateMyReviewAsync(
        int reviewId,
        UpdateBookingReviewRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> DeleteMyReviewAsync(
        int reviewId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PagedResponse<BookingReviewResponse>>> GetMyReviewsAsync(
        BookingReviewQueryRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PagedResponse<BookingReviewResponse>>> GetAllAsync(
        BookingReviewQueryRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<BookingReviewResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<BookingReviewResponse>> UpdateVisibilityAsync(
        int id,
        AdminUpdateBookingReviewVisibilityRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PublicServiceReviewSummaryResponse>> GetPublicServiceReviewsAsync(
    int serviceId,
    PublicServiceReviewQueryRequest request,
    CancellationToken cancellationToken = default);
}