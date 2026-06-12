using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Application.DTOs.Receipts;
using CarServiceBookingSystem.Application.Interfaces.IBookings;
using CarServiceBookingSystem.Application.Interfaces.IContext;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services.Bookings;

public class BookingReceiptService : IBookingReceiptService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public BookingReceiptService(ApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<BookingReceiptResponse>> GetMyReceiptAsync(
        int bookingId,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<BookingReceiptResponse>.Fail("User is not authenticated");

        var booking = await BuildBookingQuery()
            .FirstOrDefaultAsync(x =>
                x.Id == bookingId &&
                x.UserId == userId,
                cancellationToken);

        if (booking is null)
        {
            return ApiResponse<BookingReceiptResponse>.Fail("Booking not found.");
        }

        var customer = await GetCustomerAsync(booking.UserId, cancellationToken);

        return ApiResponse<BookingReceiptResponse>.Ok(
            MapToReceipt(booking, customer));
    }

    public async Task<ApiResponse<BookingReceiptResponse>> GetAdminReceiptAsync(
        int bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = await BuildBookingQuery()
            .FirstOrDefaultAsync(x => x.Id == bookingId, cancellationToken);

        if (booking is null)
        {
            return ApiResponse<BookingReceiptResponse>.Fail("Booking not found.");
        }

        var customer = await GetCustomerAsync(booking.UserId, cancellationToken);

        return ApiResponse<BookingReceiptResponse>.Ok(
            MapToReceipt(booking, customer));
    }

    private IQueryable<Booking> BuildBookingQuery()
    {
        return _context.Bookings
            .AsNoTracking()
            .Include(x => x.Car)
                .ThenInclude(x => x.CarTrim)
                    .ThenInclude(x => x.Year)
                        .ThenInclude(x => x.Model)
                            .ThenInclude(x => x.Brand)
            .Include(x => x.Service)
            .Include(x => x.ServiceBranch)
            .Include(x => x.Technician)
            .Include(x => x.Payment);
    }

    private async Task<CustomerReceiptInfo> GetCustomerAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => new
            {
                x.Id,
                x.Email,
                x.UserName,
                x.FullName
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return new CustomerReceiptInfo
            {
                UserId = userId
            };
        }

        return new CustomerReceiptInfo
        {
            UserId = user.Id,
            Name = user.FullName,
            Email = user.Email
        };
    }

    private static BookingReceiptResponse MapToReceipt(
        Booking booking,
        CustomerReceiptInfo customer)
    {
        var carTrim = booking.Car.CarTrim;
        var carYear = carTrim.Year;
        var carModel = carYear.Model;
        var carBrand = carModel.Brand;

        var durationMinutes = (int)Math.Round(
            (booking.EndDate - booking.StartDate).TotalMinutes,
            MidpointRounding.AwayFromZero);

        return new BookingReceiptResponse
        {
            BookingId = booking.Id,
            ReceiptNumber = GenerateReceiptNumber(booking.Id),
            IssuedAt = DateTime.UtcNow,
            BookingStatus = booking.Status.ToString(),

            Customer = customer,

            Car = new CarReceiptInfo
            {
                CarId = booking.CarId,
                PlateNumber = booking.Car.PlateNumber,
                DisplayName = $"{carBrand.Name} {carModel.Name} {carYear.Year} {carTrim.Name}"
            },

            Service = new ServiceReceiptInfo
            {
                ServiceId = booking.ServiceId,
                ServiceName = booking.Service.Name
            },

            Branch = booking.ServiceBranch is null
                ? null
                : new BranchReceiptInfo
                {
                    ServiceBranchId = booking.ServiceBranch.Id,
                    ServiceBranchName = booking.ServiceBranch.Name
                },

            Technician = booking.Technician is null
                ? null
                : new TechnicianReceiptInfo
                {
                    TechnicianId = booking.Technician.Id,
                    TechnicianName = booking.Technician.FullName
                },

            Schedule = new BookingScheduleReceiptInfo
            {
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                DurationMinutes = durationMinutes
            },

            Location = new BookingLocationReceiptInfo
            {
                LocationType = booking.LocationType.ToString(),
                CustomerLatitude = booking.CustomerLatitude,
                CustomerLongitude = booking.CustomerLongitude,
                CustomerCountryCode = booking.CustomerCountryCode,
                CustomerCity = booking.CustomerCity,
                CustomerFormattedAddress = booking.CustomerFormattedAddress,
                DistanceKm = (decimal?)booking.DistanceKm,
                EstimatedTravelTimeMinutes = booking.EstimatedTravelTimeMinutes
            },

            Price = new BookingPriceReceiptInfo
            {
                ServicePrice = booking.ServicePrice,
                TravelFee = booking.TravelFee,
                SubtotalPrice = booking.SubtotalPrice,
                DiscountAmount = booking.DiscountAmount,
                PromoCode = booking.PromoCodeSnapshot,
                TotalPrice = booking.TotalPrice,
                Currency = booking.Payment?.Currency ?? "aed"
            },

            Payment = booking.Payment is null
                ? null
                : new PaymentReceiptInfo
                {
                    PaymentId = booking.Payment.Id,
                    Amount = booking.Payment.Amount,
                    Currency = booking.Payment.Currency,
                    Status = booking.Payment.Status.ToString(),
                    PaidAt = booking.Payment.PaidAt,
                    StripePaymentIntentId = booking.Payment.PaymentIntentId,
                    StripeRefundId = booking.Payment.StripeRefundId,
                    RefundedAmount = booking.Payment.RefundedAmount,
                    RefundedAt = booking.Payment.RefundedAt
                }
        };
    }

    private static string GenerateReceiptNumber(int bookingId)
    {
        return $"RCPT-{DateTime.UtcNow:yyyyMMdd}-{bookingId:D6}";
    }
}