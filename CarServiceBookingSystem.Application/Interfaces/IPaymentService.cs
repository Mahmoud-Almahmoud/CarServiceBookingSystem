using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Payments;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface IPaymentService
{
    Task<ApiResponse<PaymentIntentResponse>> CreatePaymentIntentAsync(int bookingId);
}