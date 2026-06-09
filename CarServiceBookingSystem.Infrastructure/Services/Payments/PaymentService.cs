using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Payments;
using CarServiceBookingSystem.Application.Interfaces.IBookings;
using CarServiceBookingSystem.Application.Interfaces.IContext;
using CarServiceBookingSystem.Application.Interfaces.IPayments;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Payments;
using CarServiceBookingSystem.Infrastructure.Persistence;
using CarServiceBookingSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using static CarServiceBookingSystem.Application.Security.Permissions;
using StripeEvent = Stripe.Event;

namespace CarServiceBookingSystem.Infrastructure.Services.Payments;

public class PaymentService : IPaymentService
{
    private const string PaymentIntentSucceeded = "payment_intent.succeeded";
    private const string PaymentIntentPaymentFailed = "payment_intent.payment_failed";
    private const string PaymentIntentCanceled = "payment_intent.canceled";
    private const string RefundUpdated = "refund.updated";
    private const string RefundFailed = "refund.failed";
    private const string ChargeRefunded = "charge.refunded";
    private const string RefundCreated = "refund.created";

    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdempotencyContext _idempotencyContext;
    private readonly PaymentIntentService _paymentIntentService;
    private readonly StripeOptions _stripeSettings;
    private readonly ILogger<PaymentService> _logger;
    private readonly IBookingAssignmentService _bookingAssignmentService;
    private readonly string _currency;

    public PaymentService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        IOptions<StripeOptions> options,
        IIdempotencyContext idempotencyContext,
        PaymentIntentService paymentIntentService,
        ILogger<PaymentService> logger,
        IBookingAssignmentService bookingAssignmentService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _idempotencyContext = idempotencyContext;
        _paymentIntentService = paymentIntentService;
        _currency = options.Value.Currency ?? "aed";
        _stripeSettings = options.Value;
        _logger = logger;
        _bookingAssignmentService = bookingAssignmentService;
    }

    public async Task<ApiResponse<PaymentIntentResponse>> CreatePaymentIntentAsync(CreatePaymentIntentRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<PaymentIntentResponse>.Fail("User is not authenticated");

        var booking = await _context.Bookings
           .AsNoTracking()
           .Include(x => x.Service)
           .FirstOrDefaultAsync(x =>
               x.Id == request.BookingId &&
               x.UserId == userId,
               cancellationToken);

        if (booking is null)
            return ApiResponse<PaymentIntentResponse>.Fail("Booking was not found.");

        if (booking.TotalPrice <= 0)
        {
            return new ApiResponse<PaymentIntentResponse>
            {
                Success = false,
                Message = "Booking total price is invalid."
            };
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return new ApiResponse<PaymentIntentResponse>
            {
                Success = false,
                Message = "Cannot create payment for a cancelled booking."
            };
        }

        if (booking.Status == BookingStatus.Completed)
        {
            return new ApiResponse<PaymentIntentResponse>
            {
                Success = false,
                Message = "Cannot create payment for a completed booking."
            };
        }

        if (booking.TotalPrice <= 0)
        {
            return ApiResponse<PaymentIntentResponse>.Fail(
                "This booking does not require payment because the total price is zero.");
        }

        var existingPayment = await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.BookingId == booking.Id &&
                x.UserId == userId,
                cancellationToken);

        if (existingPayment is not null)
        {
            if (existingPayment.Status == PaymentStatus.Succeeded)
            {
                return new ApiResponse<PaymentIntentResponse>
                {
                    Success = false,
                    Message = "This booking is already paid."
                };
            }

            if (existingPayment.Status == PaymentStatus.Pending &&
                !string.IsNullOrWhiteSpace(existingPayment.PaymentIntentId) &&
                !string.IsNullOrWhiteSpace(existingPayment.StripeClientSecret))
            {
                return new ApiResponse<PaymentIntentResponse>
                {
                    Success = true,
                    Message = "Existing payment intent retrieved successfully.",
                    Data = new PaymentIntentResponse
                    {
                        PaymentId = existingPayment.Id,
                        BookingId = existingPayment.BookingId,
                        Amount = existingPayment.Amount,
                        Currency = existingPayment.Currency,
                        PaymentIntentId = existingPayment.PaymentIntentId,
                        ClientSecret = existingPayment.StripeClientSecret,
                        Status = existingPayment.Status
                    }
                };
            }

            return new ApiResponse<PaymentIntentResponse>
            {
                Success = false,
                Message = "A payment already exists for this booking and cannot be reused."
            };
        }

        var amountInSmallestUnit = ConvertToSmallestCurrencyUnit(
            booking.TotalPrice,
            _currency);

        var payment = new Payment
        {
            BookingId = booking.Id,
            UserId = userId,
            Amount = booking.TotalPrice,
            Currency = _currency.ToLower(),
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Payments.AddAsync(payment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var options = new PaymentIntentCreateOptions
        {
            Amount = amountInSmallestUnit,
            Currency = _currency.ToLower(),
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            },
            Metadata = new Dictionary<string, string>
            {
                ["paymentId"] = payment.Id.ToString(),
                ["bookingId"] = booking.Id.ToString(),
                ["userId"] = userId,
                ["serviceId"] = booking.ServiceId.ToString(),
                ["serviceName"] = booking.Service?.Name ?? string.Empty,
                ["source"] = "CarServiceBookingSystem"
            }
        };

        var requestOptions = new RequestOptions
        {
            IdempotencyKey = _idempotencyContext.Key
        };

        try
        {
            var paymentIntent = await _paymentIntentService.CreateAsync(
                options,
                requestOptions,
                cancellationToken);

            payment.PaymentIntentId = paymentIntent.Id;
            payment.StripeClientSecret = paymentIntent.ClientSecret;
            payment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponse<PaymentIntentResponse>
            {
                Success = true,
                Message = "Payment intent created successfully.",
                Data = new PaymentIntentResponse
                {
                    PaymentId = payment.Id,
                    BookingId = payment.BookingId,
                    Amount = payment.Amount,
                    Currency = payment.Currency,
                    PaymentIntentId = payment.PaymentIntentId!,
                    ClientSecret = payment.StripeClientSecret!,
                    Status = payment.Status
                }
            };
        }
        catch (StripeException ex)
        {
            payment.Status = PaymentStatus.Failed;
            payment.FailureReason = ex.Message;
            payment.FailureCode = ex.StripeError?.Code;
            payment.DeclineCode = ex.StripeError?.DeclineCode;
            payment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponse<PaymentIntentResponse>
            {
                Success = false,
                Message = $"Stripe payment intent creation failed: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<PaymentResponse>> ConfirmFreeBookingAsync(
    int bookingId,
    CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return ApiResponse<PaymentResponse>.Fail("User is not authenticated.");
        }

        var booking = await _context.Bookings
            .Include(x => x.Payment)
            .FirstOrDefaultAsync(x =>
                x.Id == bookingId &&
                x.UserId == userId,
                cancellationToken);

        if (booking is null)
        {
            return ApiResponse<PaymentResponse>.Fail("Booking not found.");
        }

        if (booking.TotalPrice > 0)
        {
            return ApiResponse<PaymentResponse>.Fail("This booking requires payment.");
        }

        if (booking.Status != BookingStatus.Pending)
        {
            return ApiResponse<PaymentResponse>.Fail("Only pending bookings can be confirmed as free.");
        }

        if (booking.Payment is not null)
        {
            return ApiResponse<PaymentResponse>.Fail("Payment already exists for this booking.");
        }

        var payment = new Payment
        {
            BookingId = booking.Id,
            UserId = userId,
            Amount = 0,
            Currency = "aed",
            Status = PaymentStatus.Succeeded,
            PaymentIntentId = null,
            StripeClientSecret = null,
            PaidAt = DateTime.UtcNow
        };

        booking.Payment = payment;
        booking.Status = BookingStatus.Confirmed;
        booking.UpdatedAt = DateTime.UtcNow;

        _context.Payments.Add(payment);

        await _context.SaveChangesAsync(cancellationToken);

        await _bookingAssignmentService.AutoAssignTechnicianAsync(
            booking.Id,
            cancellationToken);

        var response = await GetByBookingIdAsync(
            booking.Id,
            cancellationToken);

        return response;
    }

    public async Task<ApiResponse<string>> HandleStripeWebhookAsync(
        string json,
        string stripeSignature,
        CancellationToken cancellationToken = default)
    {
        var webhookSecret = _stripeSettings.WebhookSecret;

        if (string.IsNullOrWhiteSpace(webhookSecret))
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Stripe webhook secret is not configured."
            };
        }

        StripeEvent stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                stripeSignature,
                webhookSecret);
        }
        catch (StripeException ex)
        {
            _logger.LogWarning(ex, "Stripe webhook signature validation failed.");

            return new ApiResponse<string>
            {
                Success = false,
                Message = "Invalid Stripe webhook signature."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stripe webhook parsing failed.");

            return new ApiResponse<string>
            {
                Success = false,
                Message = "Invalid Stripe webhook payload."
            };
        }

        var alreadyProcessed = await _context.StripeWebhookEvents
            .AnyAsync(x => x.StripeEventId == stripeEvent.Id, cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogInformation(
                "Stripe webhook event {StripeEventId} was already processed.",
                stripeEvent.Id);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Stripe webhook event already processed.",
                Data = stripeEvent.Id
            };
        }

        var webhookEvent = new StripeWebhookEvent
        {
            StripeEventId = stripeEvent.Id,
            EventType = stripeEvent.Type,
            Payload = json,
            Status = WebhookProcessingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _context.StripeWebhookEvents.AddAsync(webhookEvent, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        try
        {
            switch (stripeEvent.Type)
            {
                case PaymentIntentSucceeded:
                    await HandlePaymentIntentSucceededAsync(
                        stripeEvent,
                        cancellationToken);
                    break;

                case PaymentIntentPaymentFailed:
                    await HandlePaymentIntentFailedAsync(
                        stripeEvent,
                        cancellationToken);
                    break;

                case PaymentIntentCanceled:
                    await HandlePaymentIntentCanceledAsync(
                        stripeEvent,
                        cancellationToken);
                    break;

                case RefundUpdated:
                case RefundFailed:
                    await HandleRefundEventAsync(stripeEvent, cancellationToken);
                    break;

                case ChargeRefunded:
                    await HandleChargeRefundedAsync(stripeEvent, cancellationToken);
                    break;

                default:
                    _logger.LogInformation(
                        "Unhandled Stripe webhook event type: {EventType}",
                        stripeEvent.Type);
                    break;
            }

            webhookEvent.Status = WebhookProcessingStatus.Processed;
            webhookEvent.ProcessedAt = DateTime.UtcNow;
            webhookEvent.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Stripe webhook processed successfully.",
                Data = stripeEvent.Id
            };
        }
        catch (Exception ex)
        {
            webhookEvent.Status = WebhookProcessingStatus.Failed;
            webhookEvent.ErrorMessage = ex.Message;
            webhookEvent.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogError(
                ex,
                "Stripe webhook event {StripeEventId} processing failed.",
                stripeEvent.Id);

            return new ApiResponse<string>
            {
                Success = false,
                Message = "Stripe webhook processing failed."
            };
        }
    }

    public async Task<ApiResponse<PaymentResponse>> GetByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<PaymentResponse>.Fail("User is not authenticated");

        var payment = await _context.Payments
            .AsNoTracking()
            .Include(x => x.Booking)
                .ThenInclude(x => x.Service)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.UserId == userId,
                cancellationToken);

        if (payment is null)
        {
            return new ApiResponse<PaymentResponse>
            {
                Success = false,
                Message = "Payment was not found."
            };
        }

        return new ApiResponse<PaymentResponse>
        {
            Success = true,
            Message = "Payment retrieved successfully.",
            Data = ToResponse(payment)
        };
    }

    public async Task<ApiResponse<PaymentResponse>> GetByBookingIdAsync(
        int bookingId,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<PaymentResponse>.Fail("User is not authenticated");

        var payment = await _context.Payments
            .AsNoTracking()
            .Include(x => x.Booking)
                .ThenInclude(x => x.Service)
            .FirstOrDefaultAsync(x =>
                x.BookingId == bookingId &&
                x.UserId == userId,
                cancellationToken);

        if (payment is null)
        {
            return new ApiResponse<PaymentResponse>
            {
                Success = false,
                Message = "Payment was not found."
            };
        }

        return new ApiResponse<PaymentResponse>
        {
            Success = true,
            Message = "Payment retrieved successfully.",
            Data = ToResponse(payment)
        };
    }

    public async Task<ApiResponse<PagedResponse<PaymentResponse>>> GetMyPaymentsAsync(
        PaymentFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<PagedResponse<PaymentResponse>>.Fail("User is not authenticated");

        var query = BuildPaymentQuery(request)
            .Where(x => x.UserId == userId);

        var result = await ToPagedPaymentResponseAsync(
            query,
            request,
            includeUser: false,
            cancellationToken);

        return new ApiResponse<PagedResponse<PaymentResponse>>
        {
            Success = true,
            Message = "Payments retrieved successfully.",
            Data = result
        };
    }

    public async Task<ApiResponse<PagedResponse<PaymentResponse>>> GetAllAsync(
        PaymentFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = BuildPaymentQuery(request);

        var result = await ToPagedPaymentResponseAsync(
            query,
            request,
            includeUser: true,
            cancellationToken);

        return new ApiResponse<PagedResponse<PaymentResponse>>
        {
            Success = true,
            Message = "Payments retrieved successfully.",
            Data = result
        };
    }

    public async Task<ApiResponse<PaymentResponse>> GetAdminByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var payment = await _context.Payments
            .AsNoTracking()
            .Include(x => x.Booking)
                .ThenInclude(x => x.Service)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (payment is null)
        {
            return new ApiResponse<PaymentResponse>
            {
                Success = false,
                Message = "Payment was not found."
            };
        }

        var response = ToResponse(payment);

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == payment.UserId, cancellationToken);

        response.UserEmail = user?.Email;

        return new ApiResponse<PaymentResponse>
        {
            Success = true,
            Message = "Payment retrieved successfully.",
            Data = response
        };
    }

    private static long ConvertToSmallestCurrencyUnit(decimal amount, string currency)
    {
        var normalizedCurrency = currency.Trim().ToLowerInvariant();

        var zeroDecimalCurrencies = new HashSet<string>
        {
            "bif", "clp", "djf", "gnf", "jpy", "kmf",
            "krw", "mga", "pyg", "rwf", "ugx", "vnd",
            "vuv", "xaf", "xof", "xpf"
        };

        if (zeroDecimalCurrencies.Contains(normalizedCurrency))
        {
            return (long)Math.Round(amount, 0, MidpointRounding.AwayFromZero);
        }

        return (long)Math.Round(amount * 100, 0, MidpointRounding.AwayFromZero);
    }

    private async Task HandlePaymentIntentSucceededAsync(
        StripeEvent stripeEvent,
        CancellationToken cancellationToken)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

        if (paymentIntent is null)
        {
            throw new InvalidOperationException("Stripe event object is not a PaymentIntent.");
        }

        var payment = await _context.Payments
            .Include(x => x.Booking)
            .FirstOrDefaultAsync(x =>
                x.PaymentIntentId == paymentIntent.Id,
                cancellationToken);

        if (payment is null)
        {
            throw new InvalidOperationException(
                $"Payment was not found for PaymentIntent {paymentIntent.Id}.");
        }

        if (payment.Status == PaymentStatus.Succeeded)
        {
            return;
        }

        payment.Status = PaymentStatus.Succeeded;
        payment.PaidAt = DateTime.UtcNow;
        payment.FailureReason = null;
        payment.UpdatedAt = DateTime.UtcNow;

        if (payment.Booking.Status == BookingStatus.Pending)
        {
            payment.Booking.Status = BookingStatus.Confirmed;
            payment.Booking.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _bookingAssignmentService.AutoAssignTechnicianAsync(payment.Booking.Id, cancellationToken);
    }

    private async Task HandlePaymentIntentFailedAsync(
        StripeEvent stripeEvent,
        CancellationToken cancellationToken)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

        if (paymentIntent is null)
        {
            throw new InvalidOperationException("Stripe event object is not a PaymentIntent.");
        }

        var payment = await _context.Payments
            .Include(x => x.Booking)
            .FirstOrDefaultAsync(x =>
                x.PaymentIntentId == paymentIntent.Id,
                cancellationToken);

        if (payment is null)
        {
            throw new InvalidOperationException(
                $"Payment was not found for PaymentIntent {paymentIntent.Id}.");
        }

        payment.Status = PaymentStatus.Failed;
        payment.FailureReason = paymentIntent.LastPaymentError?.Message;
        payment.DeclineCode = paymentIntent.LastPaymentError?.DeclineCode;
        payment.FailureCode = paymentIntent.LastPaymentError?.Code;
        payment.UpdatedAt = DateTime.UtcNow;

        payment.Booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task HandlePaymentIntentCanceledAsync(
        StripeEvent stripeEvent,
        CancellationToken cancellationToken)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

        if (paymentIntent is null)
        {
            throw new InvalidOperationException("Stripe event object is not a PaymentIntent.");
        }

        var payment = await _context.Payments
            .Include(x => x.Booking)
            .FirstOrDefaultAsync(x =>
                x.PaymentIntentId == paymentIntent.Id,
                cancellationToken);

        if (payment is null)
        {
            throw new InvalidOperationException(
                $"Payment was not found for PaymentIntent {paymentIntent.Id}.");
        }

        payment.Status = PaymentStatus.Cancelled;
        payment.FailureReason = "Stripe PaymentIntent was cancelled.";
        payment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleRefundEventAsync(
    Event stripeEvent,
    CancellationToken cancellationToken)
    {
        var refund = stripeEvent.Data.Object as Refund;

        if (refund is null)
        {
            return;
        }

        var payment = await FindPaymentForRefundAsync(refund, cancellationToken);

        if (payment is null)
        {
            return;
        }

        payment.StripeRefundId = refund.Id;
        payment.RefundedAmount = ConvertFromSmallestCurrencyUnit(refund.Amount, payment.Currency);
        payment.RefundFailureReason = null;

        switch (refund.Status)
        {
            case "succeeded":
                payment.Status = PaymentStatus.Refunded;
                payment.RefundedAt ??= DateTime.UtcNow;
                break;

            case "failed":
                payment.Status = PaymentStatus.RefundFailed;
                payment.RefundFailureReason = refund.FailureReason;
                break;

            case "pending":
            case "requires_action":
                payment.Status = PaymentStatus.RefundPending;
                break;

            case "canceled":
                payment.Status = PaymentStatus.RefundFailed;
                payment.RefundFailureReason = "Refund was canceled.";
                break;

            default:
                payment.Status = PaymentStatus.RefundPending;
                break;
        }

        payment.Booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleChargeRefundedAsync(
    Event stripeEvent,
    CancellationToken cancellationToken)
    {
        var charge = stripeEvent.Data.Object as Charge;

        if (charge is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(charge.PaymentIntentId))
        {
            return;
        }

        var payment = await _context.Payments
            .Include(x => x.Booking)
            .FirstOrDefaultAsync(x =>
                x.PaymentIntentId == charge.PaymentIntentId,
                cancellationToken);

        if (payment is null)
        {
            return;
        }

        var refundedAmount = ConvertFromSmallestCurrencyUnit(
            charge.AmountRefunded,
            payment.Currency);

        payment.RefundedAmount = refundedAmount;
        payment.RefundFailureReason = null;

        if (charge.Refunded || refundedAmount >= payment.Amount)
        {
            payment.Status = PaymentStatus.Refunded;
            payment.RefundedAt ??= DateTime.UtcNow;
        }
        else if (refundedAmount > 0)
        {
            payment.Status = PaymentStatus.RefundPending;
        }

        payment.Booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<Payment?> FindPaymentForRefundAsync(
    Refund refund,
    CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(refund.PaymentIntentId))
        {
            var paymentByPaymentIntent = await _context.Payments
                .Include(x => x.Booking)
                .FirstOrDefaultAsync(x =>
                    x.PaymentIntentId == refund.PaymentIntentId,
                    cancellationToken);

            if (paymentByPaymentIntent is not null)
            {
                return paymentByPaymentIntent;
            }
        }

        if (!string.IsNullOrWhiteSpace(refund.Id))
        {
            var paymentByRefundId = await _context.Payments
                .Include(x => x.Booking)
                .FirstOrDefaultAsync(x =>
                    x.StripeRefundId == refund.Id,
                    cancellationToken);

            if (paymentByRefundId is not null)
            {
                return paymentByRefundId;
            }
        }

        if (refund.Metadata is not null &&
            refund.Metadata.TryGetValue("paymentId", out var paymentIdValue) &&
            int.TryParse(paymentIdValue, out var paymentId))
        {
            return await _context.Payments
                .Include(x => x.Booking)
                .FirstOrDefaultAsync(x =>
                    x.Id == paymentId,
                    cancellationToken);
        }

        if (refund.Metadata is not null &&
            refund.Metadata.TryGetValue("bookingId", out var bookingIdValue) &&
            int.TryParse(bookingIdValue, out var bookingId))
        {
            return await _context.Payments
                .Include(x => x.Booking)
                .FirstOrDefaultAsync(x =>
                    x.BookingId == bookingId,
                    cancellationToken);
        }

        return null;
    }

    private IQueryable<Payment> BuildPaymentQuery(PaymentFilterRequest request)
    {
        var query = _context.Payments
            .AsNoTracking()
            .Include(x => x.Booking)
                .ThenInclude(x => x.Service)
            .AsQueryable();

        if (request.BookingId.HasValue)
        {
            query = query.Where(x => x.BookingId == request.BookingId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Currency))
        {
            var currency = request.Currency.Trim().ToLower();
            query = query.Where(x => x.Currency.ToLower() == currency);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= request.ToDate.Value);
        }

        if (request.MinAmount.HasValue)
        {
            query = query.Where(x => x.Amount >= request.MinAmount.Value);
        }

        if (request.MaxAmount.HasValue)
        {
            query = query.Where(x => x.Amount <= request.MaxAmount.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();

            query = query.Where(x =>
                x.Id.ToString().Contains(search) ||
                x.BookingId.ToString().Contains(search) ||
                x.Currency.ToLower().Contains(search) ||
                (x.PaymentIntentId != null &&
                 x.PaymentIntentId.ToLower().Contains(search)) ||
                (x.Booking.Service.Name != null &&
                 x.Booking.Service.Name.ToLower().Contains(search)));
        }

        query = request.SortBy?.ToLower() switch
        {
            "amount" => request.Desc
                ? query.OrderByDescending(x => x.Amount).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Amount).ThenBy(x => x.Id),

            "status" => request.Desc
                ? query.OrderByDescending(x => x.Status).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Status).ThenBy(x => x.Id),

            "paidat" => request.Desc
                ? query.OrderByDescending(x => x.PaidAt).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.PaidAt).ThenBy(x => x.Id),

            "createdat" => request.Desc
                ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

            "bookingid" => request.Desc
                ? query.OrderByDescending(x => x.BookingId).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.BookingId).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.Id)
        };

        return query;
    }

    private async Task<PagedResponse<PaymentResponse>> ToPagedPaymentResponseAsync(
        IQueryable<Payment> query,
        PaymentFilterRequest request,
        bool includeUser,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var totalRecords = await query.CountAsync(cancellationToken);

        var payments = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var responses = payments
            .Select(ToResponse)
            .ToList();

        if (includeUser && responses.Count > 0)
        {
            var userIds = payments
                .Select(x => x.UserId)
                .Distinct()
                .ToList();

            var users = await _context.Users
                .AsNoTracking()
                .Where(x => userIds.Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.Email
                })
                .ToListAsync(cancellationToken);

            foreach (var response in responses)
            {
                response.UserEmail = users
                    .FirstOrDefault(x => x.Id == response.UserId)
                    ?.Email;
            }
        }

        return new PagedResponse<PaymentResponse>
        {
            Items = responses,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalRecords
        };
    }

    private static PaymentResponse ToResponse(Payment payment)
    {
        return new PaymentResponse
        {
            Id = payment.Id,
            BookingId = payment.BookingId,
            UserId = payment.UserId,

            Amount = payment.Amount,
            Currency = payment.Currency,
            Status = payment.Status,
            StripePaymentIntentId = payment.PaymentIntentId,

            PaidAt = payment.PaidAt,
            FailureReason = payment.FailureReason,

            BookingStatus = payment.Booking.Status,
            ServiceId = payment.Booking.ServiceId,
            ServiceName = payment.Booking.Service?.Name ?? string.Empty,
            BookingStartDate = payment.Booking.StartDate,
            BookingEndDate = payment.Booking.EndDate,

            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt
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