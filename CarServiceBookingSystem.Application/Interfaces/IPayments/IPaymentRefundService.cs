using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Payments;

namespace CarServiceBookingSystem.Application.Interfaces.IPayments;

public interface IPaymentRefundService
{
    Task<ApiResponse<RefundPaymentResponse>> RefundPaymentAsync(
        int paymentId,
        RefundPaymentRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<RefundPaymentResponse>> RefundBookingPaymentAsync(
        int bookingId,
        RefundPaymentRequest request,
        CancellationToken cancellationToken = default);
}