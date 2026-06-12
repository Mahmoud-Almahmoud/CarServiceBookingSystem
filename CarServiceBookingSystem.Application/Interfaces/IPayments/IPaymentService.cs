using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Payments;

namespace CarServiceBookingSystem.Application.Interfaces.IPayments;

public interface IPaymentService
{
    Task<ApiResponse<PaymentIntentResponse>> CreatePaymentIntentAsync(CreatePaymentIntentRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<PaymentResponse>> ConfirmFreeBookingAsync(
    int bookingId,
    CancellationToken cancellationToken = default);
    Task<ApiResponse<string>> HandleStripeWebhookAsync(string json,string stripeSignature,CancellationToken cancellationToken = default);
    Task<ApiResponse<PaymentResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PaymentResponse>> GetByBookingIdAsync(
        int bookingId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PagedResponse<PaymentResponse>>> GetMyPaymentsAsync(
        PaymentFilterRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PagedResponse<PaymentResponse>>> GetAllAsync(
        PaymentFilterRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PaymentResponse>> GetAdminByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}