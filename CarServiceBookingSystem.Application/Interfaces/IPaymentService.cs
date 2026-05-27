using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Payments;
using CarServiceBookingSystem.Domain.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface IPaymentService
{
    Task<ApiResponse<PaymentIntentResponse>> CreatePaymentIntentAsync(int bookingId);
    Task<ApiResponse<WebhookProcessingStatus>> HandleStripeWebhookAsync(StripeWebhookDto stripeWebhookDto);
}