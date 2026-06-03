using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Application.DTOs.Payments;
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
    private readonly IEmailService _emailService;
    private readonly IBookingAvailabilityService _bookingAvailabilityService;
    private readonly IBookingQuoteService _bookingQuoteService;
    private readonly IPaymentRefundService _paymentRefundService;
    private readonly IBookingAssignmentService _bookingAssignmentService;

    public BookingService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService, 
        IBackgroundJobService backgroundJobService, 
        IEmailService emailService, 
        IBookingAvailabilityService bookingAvailabilityService,
        IBookingQuoteService bookingQuoteService,
        IPaymentRefundService paymentRefundService,
        IBookingAssignmentService bookingAssignmentService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _backgroundJobService = backgroundJobService;
        _emailService = emailService;
        _bookingAvailabilityService = bookingAvailabilityService;
        _bookingQuoteService = bookingQuoteService;
        _paymentRefundService = paymentRefundService;
        _bookingAssignmentService = bookingAssignmentService;
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
            .Include (x => x.Technician)
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

    public async Task<ApiResponse<BookingCancellationResponse>> CancelMyBookingAsync(
    int bookingId,
    CancelBookingRequest request,
    CancellationToken cancellationToken = default)
    {

        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<BookingCancellationResponse>.Fail("User is not authenticated");

        var booking = await _context.Bookings
            .Include(x => x.Payment)
            .FirstOrDefaultAsync(x =>
                x.Id == bookingId &&
                x.UserId == userId,
                cancellationToken);

        if (booking is null)
        {
            return ApiResponse<BookingCancellationResponse>.Fail("Booking not found.");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return ApiResponse<BookingCancellationResponse>.Fail("Booking is already cancelled.");
        }

        if (booking.Status == BookingStatus.InProgress)
        {
            return ApiResponse<BookingCancellationResponse>.Fail("Booking cannot be cancelled after work has started.");
        }

        if (booking.Status == BookingStatus.Completed)
        {
            return ApiResponse<BookingCancellationResponse>.Fail("Completed bookings cannot be cancelled.");
        }

        if (booking.StartDate <= DateTime.UtcNow)
        {
            return ApiResponse<BookingCancellationResponse>.Fail("Past or already-started bookings cannot be cancelled.");
        }

        var canCancel =
            booking.Status == BookingStatus.Pending ||
            booking.Status == BookingStatus.Confirmed ||
            booking.Status == BookingStatus.Assigned;

        if (!canCancel)
        {
            return ApiResponse<BookingCancellationResponse>.Fail(
                $"Booking with status {booking.Status} cannot be cancelled by the user.");
        }

        booking.Status = BookingStatus.Cancelled;
        booking.CancelledAt = DateTime.UtcNow;
        booking.CancelledByUserId = userId;
        booking.CancellationReason = string.IsNullOrWhiteSpace(request.Reason)
            ? null
            : request.Reason.Trim();
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var refundRequired =
            booking.Payment is not null &&
            booking.Payment.Status == PaymentStatus.Succeeded;

        if (refundRequired)
        {
            await _paymentRefundService.RefundBookingPaymentAsync(
                booking.Id,
                new RefundPaymentRequest
                {
                    Reason = booking.CancellationReason
                },
                cancellationToken);
        }

        await _context.Entry(booking)
            .Reference(x => x.Payment)
            .LoadAsync(cancellationToken);

        var response = new BookingCancellationResponse
        {
            BookingId = booking.Id,
            Status = booking.Status.ToString(),
            CancelledAt = booking.CancelledAt,
            CancellationReason = booking.CancellationReason,
            RefundRequired = refundRequired,
            PaymentStatus = booking.Payment?.Status.ToString()
        };

        return ApiResponse<BookingCancellationResponse>.Ok(response);
    }

    public async Task<ApiResponse<RescheduleBookingResponse>> RescheduleMyBookingAsync(
    int bookingId,
    RescheduleBookingRequest request,
    CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<RescheduleBookingResponse>.Fail("User is not authenticated");

        var booking = await _context.Bookings
            .Include(x => x.ServiceBranch)
            .Include(x => x.Technician)
            .FirstOrDefaultAsync(x =>
                x.Id == bookingId &&
                x.UserId == userId,
                cancellationToken);

        if (booking is null)
        {
            return ApiResponse<RescheduleBookingResponse>.Fail("Booking not found.");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return ApiResponse<RescheduleBookingResponse>.Fail("Cancelled bookings cannot be rescheduled.");
        }

        if (booking.Status == BookingStatus.Completed)
        {
            return ApiResponse<RescheduleBookingResponse>.Fail("Completed bookings cannot be rescheduled.");
        }

        if (booking.Status == BookingStatus.InProgress)
        {
            return ApiResponse<RescheduleBookingResponse>.Fail("Bookings in progress cannot be rescheduled.");
        }

        if (booking.StartDate <= DateTime.UtcNow)
        {
            return ApiResponse<RescheduleBookingResponse>.Fail("Past or already-started bookings cannot be rescheduled.");
        }

        var canReschedule =
            booking.Status == BookingStatus.Pending ||
            booking.Status == BookingStatus.Confirmed ||
            booking.Status == BookingStatus.Assigned;

        if (!canReschedule)
        {
            return ApiResponse<RescheduleBookingResponse>.Fail(
                $"Booking with status {booking.Status} cannot be rescheduled.");
        }

        var quoteRequest = new BookingQuoteRequest
        {
            CarId = booking.CarId,
            ServiceId = booking.ServiceId,
            StartDate = request.StartDate,
            LocationType = booking.LocationType,
            ServiceBranchId = request.ServiceBranchId ?? booking.ServiceBranchId,
            CustomerLatitude = request.CustomerLatitude ?? booking.CustomerLatitude,
            CustomerLongitude = request.CustomerLongitude ?? booking.CustomerLongitude,
            CustomerCountryCode = request.CustomerCountryCode ?? booking.CustomerCountryCode,
            CustomerCity = request.CustomerCity ?? booking.CustomerCity
        };

        var quoteResponse = await _bookingQuoteService.GetQuoteAsync(
            quoteRequest,
            cancellationToken);

        if (!quoteResponse.Success || quoteResponse.Data is null)
        {
            return ApiResponse<RescheduleBookingResponse>.Fail(
                quoteResponse.Message ?? "Unable to calculate booking quote.");
        }

        var quote = quoteResponse.Data;

        if (!quote.IsAvailable)
        {
            return ApiResponse<RescheduleBookingResponse>.Fail(
                quote.UnavailableReason ?? "The selected slot is not available.");
        }

        booking.StartDate = request.StartDate;
        booking.EndDate = quote.EndDate;

        booking.ServicePrice = quote.ServicePrice;
        booking.TravelFee = quote.TravelFee;
        booking.TotalPrice = quote.TotalPrice;

        booking.CustomerLatitude = request.CustomerLatitude ?? booking.CustomerLatitude;
        booking.CustomerLongitude = request.CustomerLongitude ?? booking.CustomerLongitude;
        booking.CustomerCountryCode = quote.CustomerCountryCode;
        booking.CustomerCity = quote.CustomerCity;
        booking.CustomerFormattedAddress = quote.CustomerFormattedAddress;

        booking.DistanceKm = quote.DistanceKm;
        booking.EstimatedTravelTimeMinutes = quote.EstimatedTravelTimeMinutes;

        booking.ServicePriceRuleId = quote.ServicePriceRuleId;
        booking.ServiceAreaRuleId = quote.MatchedServiceAreaRuleId;
        booking.ServiceBranchId = quote.ServiceBranchId;

        booking.TechnicianId = null;

        if (booking.Status == BookingStatus.Assigned)
        {
            booking.Status = BookingStatus.Confirmed;
        }

        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var assignmentResponse = await _bookingAssignmentService.AutoAssignTechnicianAsync(
            booking.Id,
            cancellationToken);

        var updatedBooking = await _context.Bookings
            .AsNoTracking()
            .Include(x => x.ServiceBranch)
            .Include(x => x.Technician)
            .FirstOrDefaultAsync(x => x.Id == booking.Id, cancellationToken);

        if (updatedBooking is null)
        {
            return ApiResponse<RescheduleBookingResponse>.Fail("Booking not found after reschedule.");
        }

        var response = new RescheduleBookingResponse
        {
            BookingId = updatedBooking.Id,
            StartDate = updatedBooking.StartDate,
            EndDate = updatedBooking.EndDate,
            ServiceBranchId = updatedBooking.ServiceBranchId,
            ServiceBranchName = updatedBooking.ServiceBranch?.Name,
            TechnicianId = updatedBooking.TechnicianId,
            TechnicianName = updatedBooking.Technician?.FullName,
            ServicePrice = updatedBooking.ServicePrice,
            TravelFee = updatedBooking.TravelFee,
            TotalPrice = updatedBooking.TotalPrice,
            Status = updatedBooking.Status.ToString(),
            TechnicianReassigned = assignmentResponse.Success && updatedBooking.TechnicianId.HasValue
        };

        return ApiResponse<RescheduleBookingResponse>.Ok(response);
    }

    private IQueryable<Booking> GetBookingQuery()
    {
        return _context.Bookings
            .AsNoTracking()
            .Include(x => x.Car)
            .Include(x => x.Service)
            .Include(x => x.ServiceBranch)
            .Include(x => x.Technician);
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
            UserId = booking.UserId,
            PlateNumber = booking.Car.PlateNumber,
            ServiceId = booking.ServiceId,
            ServiceBranchId = booking.ServiceBranchId,
            ServiceBranchName = booking.ServiceBranch?.Name,
            ServiceName = booking.Service?.Name ?? string.Empty,
            LocationType = booking.LocationType,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            Status = booking.Status,

            TechnicianId = booking.TechnicianId,
            TechnicianName = booking.Technician?.FullName ?? string.Empty,

            ServicePrice = booking.ServicePrice,
            TravelFee = booking.TravelFee,
            TotalPrice = booking.TotalPrice,

            CustomerLatitude = booking.CustomerLatitude,
            CustomerLongitude = booking.CustomerLongitude,
            CustomerCountryCode = booking.CustomerCountryCode,
            CustomerCity = booking.CustomerCity,
            CustomerFormattedAddress = booking.CustomerFormattedAddress,

            DistanceKm = booking.DistanceKm,
            EstimatedTravelTimeMinutes = booking.EstimatedTravelTimeMinutes,

            ServicePriceRuleId = booking.ServicePriceRuleId,
            ServiceAreaRuleId = booking.ServiceAreaRuleId,

            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt
        };
    }
}