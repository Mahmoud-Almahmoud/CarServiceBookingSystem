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
            return ApiResponse<RefundPaymentResponse>.Fail("Payment is already fully refunded.");
        }

        if (payment.Status != PaymentStatus.Succeeded &&
            payment.Status != PaymentStatus.PartiallyRefunded)
        {
            return ApiResponse<RefundPaymentResponse>.Fail("Only succeeded payments can be refunded.");
        }

        if (string.IsNullOrWhiteSpace(payment.PaymentIntentId))
        {
            return ApiResponse<RefundPaymentResponse>.Fail("Payment does not have a Stripe PaymentIntent ID.");
        }

        var alreadyRefundedAmount = payment.RefundedAmount ?? 0m;
        var remainingRefundableAmount = payment.Amount - alreadyRefundedAmount;

        if (remainingRefundableAmount <= 0)
        {
            return ApiResponse<RefundPaymentResponse>.Fail("Payment has no remaining refundable amount.");
        }

        var requestedRefundAmount = request.Amount ?? remainingRefundableAmount;

        if (requestedRefundAmount <= 0)
        {
            return ApiResponse<RefundPaymentResponse>.Fail("Refund amount must be greater than zero.");
        }

        if (requestedRefundAmount > remainingRefundableAmount)
        {
            return ApiResponse<RefundPaymentResponse>.Fail(
                $"Refund amount cannot exceed remaining refundable amount: {remainingRefundableAmount} {payment.Currency}.");
        }

        try
        {
            var refundService = new RefundService();

            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = payment.PaymentIntentId,
                Amount = ConvertToSmallestCurrencyUnit(requestedRefundAmount, payment.Currency),
                Reason = "requested_by_customer",
                Metadata = new Dictionary<string, string>
                {
                    ["paymentId"] = payment.Id.ToString(),
                    ["bookingId"] = payment.BookingId.ToString(),
                    ["reason"] = request.Reason ?? string.Empty,
                    ["requestedRefundAmount"] = requestedRefundAmount.ToString("0.00")
                }
            };

            var requestOptions = new RequestOptions
            {
                IdempotencyKey = $"booking-payment-refund-{payment.BookingId}-{requestedRefundAmount:0.00}"
            };

            var refund = await refundService.CreateAsync(
                refundOptions,
                requestOptions,
                cancellationToken);

            payment.StripeRefundId = refund.Id;

            var stripeRefundedAmount = ConvertFromSmallestCurrencyUnit(refund.Amount, payment.Currency);
            var totalRefundedAmount = alreadyRefundedAmount + stripeRefundedAmount;

            payment.RefundedAmount = totalRefundedAmount > payment.Amount
                ? payment.Amount
                : totalRefundedAmount;

            payment.RefundFailureReason = null;

            if (refund.Status == "succeeded")
            {
                if (payment.RefundedAmount >= payment.Amount)
                {
                    payment.Status = PaymentStatus.Refunded;
                    payment.RefundedAt ??= DateTime.UtcNow;
                }
                else
                {
                    payment.Status = PaymentStatus.PartiallyRefunded;
                }
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

    private static long ConvertToSmallestCurrencyUnit(decimal amount, string currency)
    {
        var zeroDecimalCurrencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "bif", "clp", "djf", "gnf", "jpy", "kmf", "krw",
        "mga", "pyg", "rwf", "ugx", "vnd", "vuv", "xaf",
        "xof", "xpf"
    };

        return zeroDecimalCurrencies.Contains(currency)
            ? (long)Math.Round(amount, 0, MidpointRounding.AwayFromZero)
            : (long)Math.Round(amount * 100m, 0, MidpointRounding.AwayFromZero);
    }
}