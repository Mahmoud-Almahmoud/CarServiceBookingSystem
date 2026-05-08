using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBackgroundJobService _backgroundJobService;

    public BookingService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService, IBackgroundJobService backgroundJobService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _backgroundJobService = backgroundJobService;
    }

    public async Task<ApiResponse<BookingResponse>> CreateAsync(CreateBookingRequest request)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<BookingResponse>.Fail("User is not authenticated");

        var car = await _context.Cars
            .FirstOrDefaultAsync(x => x.Id == request.CarId && x.UserId == userId);

        if (car == null)
            return ApiResponse<BookingResponse>.Fail("Car not found");

        var service = await _context.Services
            .FirstOrDefaultAsync(x => x.Id == request.ServiceId);

        if (service == null)
            return ApiResponse<BookingResponse>.Fail("Service not found");

        if (request.StartDate <= DateTime.UtcNow)
            return ApiResponse<BookingResponse>.Fail("Start date must be in the future");

        var booking = new Booking
        {
            UserId = userId,
            CarId = request.CarId,
            ServiceId = request.ServiceId,
            LocationType = request.LocationType,
            StartDate = request.StartDate,
            EndDate = request.StartDate.AddMinutes(service.DurationInMinutes),
            Status = BookingStatus.Pending
        };

        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        var response = await BuildBookingResponseAsync(booking.Id);

        return ApiResponse<BookingResponse>.Ok(response!, "Booking created successfully");
    }

    public async Task<ApiResponse<List<BookingResponse>>> GetMyBookingsAsync()
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<List<BookingResponse>>.Fail("User is not authenticated");

        var bookings = await GetBookingQuery()
            .Where(x => x.UserId == userId)
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
            .Include(x => x.Service);
    }

    private async Task<BookingResponse?> BuildBookingResponseAsync(int bookingId)
    {
        return await GetBookingQuery()
            .Where(x => x.Id == bookingId)
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
            ServiceName = booking.Service.Name,
            ServicePrice = booking.Service.Price,
            LocationType = booking.LocationType,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            Status = booking.Status
        };
    }
}