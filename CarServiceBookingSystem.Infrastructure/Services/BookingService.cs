using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using static CarServiceBookingSystem.Application.Security.Permissions;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly IEmailService _emailService;
    private readonly IBookingAvailabilityService _bookingAvailabilityService;
    private readonly IBookingQuoteService _bookingQuoteService;

    public BookingService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService, 
        IBackgroundJobService backgroundJobService, 
        IEmailService emailService, 
        IBookingAvailabilityService bookingAvailabilityService,
        IBookingQuoteService bookingQuoteService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _backgroundJobService = backgroundJobService;
        _emailService = emailService;
        _bookingAvailabilityService = bookingAvailabilityService;
        _bookingQuoteService = bookingQuoteService;
    }
    
    public async Task<ApiResponse<BookingResponse>> CreateAsync(CreateBookingRequest request,CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<BookingResponse>.Fail("User is not authenticated");

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var quoteResponse = await _bookingQuoteService.GetQuoteAsync(
            new BookingQuoteRequest
            {
                CarId = request.CarId,
                ServiceId = request.ServiceId,
                ServiceBranchId = request.ServiceBranchId,
                LocationType = request.LocationType,
                StartDate = request.StartDate,
                CustomerLatitude = request.CustomerLatitude,
                CustomerLongitude = request.CustomerLongitude,
                CustomerCountryCode = request.CustomerCountryCode,
                CustomerCity = request.CustomerCity
            },cancellationToken);

        if (!quoteResponse.Success || quoteResponse.Data is null)
        {
            return new ApiResponse<BookingResponse>
            {
                Success = false,
                Message = quoteResponse.Message
            };
        }

        var quote = quoteResponse.Data;

        if (!quote.IsAvailable)
        {
            return new ApiResponse<BookingResponse>
            {
                Success = false,
                Message = quote.UnavailableReason ?? "Selected booking option is not available."
            };
        }

        var booking = new Booking
        {
            UserId = userId,
            CarId = request.CarId,
            ServiceId = request.ServiceId,
            ServiceBranchId = quote.ServiceBranchId,
            LocationType = request.LocationType,

            StartDate = quote.StartDate,
            EndDate = quote.EndDate,
            Status = BookingStatus.Pending,

            ServicePrice = quote.ServicePrice,
            TravelFee = quote.TravelFee,
            TotalPrice = quote.TotalPrice,

            CustomerLatitude = quote.CustomerLatitude,
            CustomerLongitude = quote.CustomerLongitude,
            CustomerCountryCode = quote.CustomerCountryCode,
            CustomerCity = quote.CustomerCity,

            DistanceKm = quote.DistanceKm.HasValue
                ? quote.DistanceKm.Value
                : null,

            EstimatedTravelTimeMinutes = quote.EstimatedTravelTimeMinutes,

            ServicePriceRuleId = quote.ServicePriceRuleId,
            ServiceAreaRuleId = quote.MatchedServiceAreaRuleId,

            CreatedAt = DateTime.UtcNow
        };

        await _context.Bookings.AddAsync(booking, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        var createdBooking = await _context.Bookings
            .AsNoTracking()
            .Include(x => x.Service)
            .Include(x => x.Car)
            .Include(x => x.ServiceBranch)
            .FirstAsync(x => x.Id == booking.Id, cancellationToken);

        return new ApiResponse<BookingResponse>
        {
            Success = true,
            Message = "Booking created successfully.",
            Data = ToResponse(createdBooking)
        };
    }

    public async Task<ApiResponse<List<BookingResponse>>> GetMyBookingsAsync()
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<List<BookingResponse>>.Fail("User is not authenticated");

        var bookings = await GetBookingQuery()
            .Where(x => x.UserId == userId)
            .Include(x => x.Service)
            .Include(x => x.Car)
            .OrderByDescending(x => x.StartDate)
            .Select(x => ToResponse(x))
            .ToListAsync();

        return ApiResponse<List<BookingResponse>>.Ok(bookings);
    }

    public async Task<ApiResponse<PagedResponse<BookingResponse>>> GetAllAsync(
    BookingQueryRequest request)
    {
        var query = GetBookingQuery();

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.Car.PlateNumber.Contains(search) ||
                x.Service.Name.Contains(search));
        }

        query = request.SortBy?.ToLower() switch
        {
            "startdate" => request.Desc
                ? query.OrderByDescending(x => x.StartDate).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.StartDate).ThenBy(x => x.Id),

            "status" => request.Desc
                ? query.OrderByDescending(x => x.Status).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Status).ThenBy(x => x.Id),

            "price" => request.Desc
                ? query.OrderByDescending(x => x.Service.Price).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Service.Price).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.Id)
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Include(x => x.Service)
            .Include(x => x.Car)
            .Select(x => ToResponse(x))
            .ToListAsync();

        var response = new PagedResponse<BookingResponse>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        return ApiResponse<PagedResponse<BookingResponse>>.Ok(response);
    }

    public async Task<ApiResponse<BookingResponse>> UpdateStatusAsync(
        int bookingId,
        UpdateBookingStatusRequest request)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(x => x.Id == bookingId);

        if (booking == null)
            return ApiResponse<BookingResponse>.Fail("Booking not found");

        booking.Status = request.Status;

        await _context.SaveChangesAsync();

        if (booking.Status == BookingStatus.Confirmed)
        {
            var userEmail = await _context.Users
                .Where(x => x.Id == booking.UserId)
                .Select(x => x.Email)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                _backgroundJobService.EnqueueEmail(
                    userEmail,
                    "Booking Confirmed",
                    $"""
                    Your booking has been confirmed.

                    Booking ID: {booking.Id}
                    Start Date: {booking.StartDate:yyyy-MM-dd HH:mm}
                    End Date: {booking.EndDate:yyyy-MM-dd HH:mm}
                    Status: {booking.Status}
                    """);
            }
        }

        var response = await BuildBookingResponseAsync(booking.Id);

        return ApiResponse<BookingResponse>.Ok(response!, "Booking status updated successfully");
    }

    private IQueryable<Booking> GetBookingQuery()
    {
        return _context.Bookings
            .AsNoTracking()
            .Include(x => x.Car)
            .Include(x => x.Service)
            .Include(x => x.ServiceBranch);
    }

    private async Task<BookingResponse?> BuildBookingResponseAsync(int bookingId)
    {
        return await GetBookingQuery()
            .Where(x => x.Id == bookingId)
            .Include(x => x.Service)
            .Include(x => x.Car)
            .Include(x => x.ServiceBranch)
            .Select(x => ToResponse(x))
            .FirstOrDefaultAsync();
    }

    private static BookingResponse ToResponse(Booking booking)
    {
        return new BookingResponse
        {
            Id = booking.Id,
            CarId = booking.CarId,
            PlateNumber = booking.Car.PlateNumber,
            ServiceId = booking.ServiceId,
            ServiceBranchId = booking.ServiceBranchId,
            ServiceBranchName = booking.ServiceBranch?.Name,
            ServiceName = booking.Service?.Name ?? string.Empty,
            LocationType = booking.LocationType,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            Status = booking.Status,

            ServicePrice = booking.ServicePrice,
            TravelFee = booking.TravelFee,
            TotalPrice = booking.TotalPrice,

            CustomerLatitude = booking.CustomerLatitude,
            CustomerLongitude = booking.CustomerLongitude,
            CustomerCountryCode = booking.CustomerCountryCode,
            CustomerCity = booking.CustomerCity,

            DistanceKm = booking.DistanceKm,
            EstimatedTravelTimeMinutes = booking.EstimatedTravelTimeMinutes,

            ServicePriceRuleId = booking.ServicePriceRuleId,
            ServiceAreaRuleId = booking.ServiceAreaRuleId,
        };
    }
}