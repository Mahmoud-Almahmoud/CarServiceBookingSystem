using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Payments;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class PaymentRefundService : IPaymentRefundService
{
    private readonly ApplicationDbContext _context;

    public PaymentRefundService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<RefundPaymentResponse>> RefundBookingPaymentAsync(
        int bookingId,
        RefundPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var payment = await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BookingId == bookingId, cancellationToken);

        if (payment is null)
        {
            return ApiResponse<RefundPaymentResponse>.Fail("Payment not found for this booking.");
        }

        return await RefundPaymentAsync(payment.Id, request, cancellationToken);
    }

    public async Task<ApiResponse<RefundPaymentResponse>> RefundPaymentAsync(
        int paymentId,
        RefundPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var payment = await _context.Payments
            .Include(x => x.Booking)
            .FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken);

        if (payment is null)
        {
            return ApiResponse<RefundPaymentResponse>.Fail("Payment not found.");
        }

        if (payment.Status == PaymentStatus.Refunded)
        {
            return ApiResponse<RefundPaymentResponse>.Fail("Payment is already refunded.");
        }

        if (payment.Status != PaymentStatus.Succeeded)
        {
            return ApiResponse<RefundPaymentResponse>.Fail("Only succeeded payments can be refunded.");
        }

        if (string.IsNullOrWhiteSpace(payment.PaymentIntentId))
        {
            return ApiResponse<RefundPaymentResponse>.Fail("Payment does not have a Stripe PaymentIntent ID.");
        }

        try
        {
            var refundService = new RefundService();

            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = payment.PaymentIntentId,
                Reason = "requested_by_customer",
                Metadata = new Dictionary<string, string>
                {
                    ["paymentId"] = payment.Id.ToString(),
                    ["bookingId"] = payment.BookingId.ToString(),
                    ["reason"] = request.Reason ?? string.Empty
                }
            };

            var requestOptions = new RequestOptions
            {
                IdempotencyKey = $"booking-payment-refund-{payment.BookingId}"
            };

            var refund = await refundService.CreateAsync(
                refundOptions,
                requestOptions,
                cancellationToken);

            payment.StripeRefundId = refund.Id;
            payment.RefundedAmount = ConvertFromSmallestCurrencyUnit(refund.Amount, payment.Currency);
            payment.RefundedAt = DateTime.UtcNow;
            payment.RefundFailureReason = null;

            if (refund.Status == "succeeded")
            {
                payment.Status = PaymentStatus.Refunded;
            }
            else
            {
                payment.Status = PaymentStatus.RefundPending;
            }

            payment.Booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<RefundPaymentResponse>.Ok(MapToResponse(payment));
        }
        catch (StripeException ex)
        {
            payment.Status = PaymentStatus.RefundFailed;
            payment.RefundFailureReason = ex.Message;

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<RefundPaymentResponse>.Fail($"Stripe refund failed: {ex.Message}");
        }
    }

    private static RefundPaymentResponse MapToResponse(Domain.Entities.Payment payment)
    {
        return new RefundPaymentResponse
        {
            PaymentId = payment.Id,
            BookingId = payment.BookingId,
            Amount = payment.Amount,
            RefundedAmount = payment.RefundedAmount ?? 0,
            Currency = payment.Currency,
            PaymentStatus = payment.Status.ToString(),
            StripePaymentIntentId = payment.PaymentIntentId,
            StripeRefundId = payment.StripeRefundId,
            RefundedAt = payment.RefundedAt
        };
    }

    private static decimal ConvertFromSmallestCurrencyUnit(long amount, string currency)
    {
        var zeroDecimalCurrencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "bif", "clp", "djf", "gnf", "jpy", "kmf", "krw",
            "mga", "pyg", "rwf", "ugx", "vnd", "vuv", "xaf",
            "xof", "xpf"
        };

        return zeroDecimalCurrencies.Contains(currency)
            ? amount
            : amount / 100m;
    }
}