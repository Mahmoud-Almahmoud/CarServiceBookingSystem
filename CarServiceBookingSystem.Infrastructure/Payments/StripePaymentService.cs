using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Payments;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;

namespace CarServiceBookingSystem.Infrastructure.Payments;

public class StripePaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly StripeSettings _settings;
    private readonly IIdempotencyContext _idempotencyContext;

    public StripePaymentService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        IOptions<StripeSettings> options,
        IIdempotencyContext idempotencyContext)
    {
        _context = context;
        _currentUserService = currentUserService;
        _settings = options.Value;
        _idempotencyContext = idempotencyContext;
    }

    public async Task<ApiResponse<PaymentIntentResponse>> CreatePaymentIntentAsync(int bookingId)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<PaymentIntentResponse>.Fail("User is not authenticated");

        var booking = await _context.Bookings
            .Include(x => x.Service)
            .Include(x => x.Payment)
            .FirstOrDefaultAsync(x => x.Id == bookingId && x.UserId == userId);

        if (booking == null)
            return ApiResponse<PaymentIntentResponse>.Fail("Booking not found");

        if (booking.Payment != null && booking.Payment.Status == PaymentStatus.Succeeded)
            return ApiResponse<PaymentIntentResponse>.Fail("Booking is already paid");

        StripeConfiguration.ApiKey = _settings.SecretKey;

        var amountInSmallestUnit = (long)(booking.Service.Price * 100);

        var options = new PaymentIntentCreateOptions
        {
            Amount = amountInSmallestUnit,
            Currency = _settings.Currency,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            },
            Metadata = new Dictionary<string, string>
            {
                { "bookingId", booking.Id.ToString() },
                { "userId", userId }
            }
        };

        var service = new PaymentIntentService();
        var requestOptions = new RequestOptions
        {
            IdempotencyKey = _idempotencyContext.Key
        };

        var paymentIntent = await service.CreateAsync(options,requestOptions);

        var payment = booking.Payment;

        if (payment == null)
        {
            payment = new Payment
            {
                BookingId = booking.Id,
                Amount = booking.Service.Price,
                PaymentIntentId = paymentIntent.Id,
                Status = PaymentStatus.Pending
            };

            await _context.Payments.AddAsync(payment);
        }
        else
        {
            payment.PaymentIntentId = paymentIntent.Id;
            payment.Amount = booking.Service.Price;
            payment.Status = PaymentStatus.Pending;
        }

        await _context.SaveChangesAsync();

        return ApiResponse<PaymentIntentResponse>.Ok(new PaymentIntentResponse
        {
            BookingId = booking.Id,
            PaymentIntentId = paymentIntent.Id,
            ClientSecret = paymentIntent.ClientSecret,
            Amount = booking.Service.Price,
            Currency = _settings.Currency
        }, "Payment intent created successfully");
    }

    public async Task<ApiResponse<WebhookProcessingStatus>> HandleStripeWebhookAsync(StripeWebhookDto stripeWebhookDto)
    {
        var alreadyProcessed = await _context.StripeWebhookEvents
                .AnyAsync(x => x.StripeEventId == stripeWebhookDto.EventId && x.Processed);

        if (alreadyProcessed)
            return ApiResponse<WebhookProcessingStatus>.Ok(WebhookProcessingStatus.AlreadyProcessed);

        var webhookEvent = new StripeWebhookEvent
        {
            StripeEventId = stripeWebhookDto.EventId,
            EventType = stripeWebhookDto.EventType,
            Processed = false
        };

        _context.StripeWebhookEvents.Add(webhookEvent);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ApiResponse<WebhookProcessingStatus>.Ok(WebhookProcessingStatus.AlreadyProcessed);
        }

        switch (stripeWebhookDto.EventType)
        {
            case "payment_intent.succeeded":
                var paymentIntent = stripeWebhookDto.PaymentIntentId != null ? new PaymentIntent { Id = stripeWebhookDto.PaymentIntentId } : null;

                if (paymentIntent is null)
                    return ApiResponse<WebhookProcessingStatus>.Ok(WebhookProcessingStatus.Invalid);

                await HandlePaymentIntentSucceededAsync(paymentIntent.Id);
                break;

            case "payment_intent.payment_failed":
                var failedPaymentIntent = stripeWebhookDto.PaymentIntentId != null ? new PaymentIntent { Id = stripeWebhookDto.PaymentIntentId } : null;

                if (failedPaymentIntent is null)
                    return ApiResponse<WebhookProcessingStatus>.Ok(WebhookProcessingStatus.Invalid);

                await HandlePaymentIntentFailedAsync(failedPaymentIntent.Id);
                break;

            default:
                webhookEvent.Processed = true;
                webhookEvent.ProcessedAtUtc = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse<WebhookProcessingStatus>.Ok(WebhookProcessingStatus.Ignored);
        }

        webhookEvent.Processed = true;
        webhookEvent.ProcessedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ApiResponse<WebhookProcessingStatus>.Ok(WebhookProcessingStatus.Processed);
    }

    private async Task HandlePaymentIntentSucceededAsync(string paymentIntentId)
    {
        var payment = await _context.Payments
            .Include(x => x.Booking)
            .FirstOrDefaultAsync(x => x.PaymentIntentId == paymentIntentId);

        if (payment is null)
            return;

        if (payment.Status == PaymentStatus.Succeeded)
            return;

        payment.Status = PaymentStatus.Succeeded;
        payment.PaidAt = DateTime.UtcNow;

        payment.Booking.Status = BookingStatus.Confirmed;
    }

    private async Task HandlePaymentIntentFailedAsync(string paymentIntentId)
    {
        var payment = await _context.Payments
            .Include(x => x.Booking)
            .FirstOrDefaultAsync(x => x.PaymentIntentId == paymentIntentId);

        if (payment is null)
            return;

        if (payment.Status == PaymentStatus.Failed)
            return;

        payment.Status = PaymentStatus.Failed;

        payment.Booking.Status = BookingStatus.Pending;
    }
}