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

    public StripePaymentService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        IOptions<StripeSettings> options)
    {
        _context = context;
        _currentUserService = currentUserService;
        _settings = options.Value;
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
        var paymentIntent = await service.CreateAsync(options);

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
}