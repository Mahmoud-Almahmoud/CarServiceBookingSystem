using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Options;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class BookingAvailabilityService : IBookingAvailabilityService
{
    private readonly ApplicationDbContext _context;
    private readonly BookingAvailabilityOptions _options;

    private static readonly BookingStatus[] BlockingBookingStatuses =
    [
        BookingStatus.Pending,
        BookingStatus.Confirmed
    ];

    public BookingAvailabilityService(
        ApplicationDbContext context,
        IOptions<BookingAvailabilityOptions> options)
    {
        _context = context;
        _options = options.Value;
    }

    public async Task<ApiResponse<List<AvailableSlotResponse>>> GetAvailableSlotsAsync(
        AvailableSlotsRequest request,
        CancellationToken cancellationToken = default)
    {
        var date = request.Date.Date;

        if (date < DateTime.UtcNow.Date)
        {
            return new ApiResponse<List<AvailableSlotResponse>>
            {
                Success = false,
                Message = "Date cannot be in the past.",
                Data = []
            };
        }

        var branchOffersService = await _context.BranchServices
            .AsNoTracking()
            .AnyAsync(x =>
                x.ServiceBranchId == request.ServiceBranchId &&
                x.ServiceId == request.ServiceId &&
                x.IsActive &&
                x.ServiceBranch.IsActive,
                cancellationToken);

        if (!branchOffersService)
        {
            return new ApiResponse<List<AvailableSlotResponse>>
            {
                Success = false,
                Message = "The selected branch does not offer this service.",
                Data = []
            };
        }

        var service = await _context.Services
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.ServiceId && x.IsActive, cancellationToken);

        if (service is null)
        {
            return new ApiResponse<List<AvailableSlotResponse>>
            {
                Success = false,
                Message = "Service was not found.",
                Data = []
            };
        }

        var workingHour = await _context.BranchWorkingHours
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.ServiceBranchId == request.ServiceBranchId &&
                x.DayOfWeek == date.DayOfWeek,
                cancellationToken);

        if (workingHour is null || workingHour.IsClosed)
        {
            return new ApiResponse<List<AvailableSlotResponse>>
            {
                Success = true,
                Message = "Branch is closed on this date.",
                Data = []
            };
        }

        var serviceDuration = TimeSpan.FromMinutes(service.DurationInMinutes);

        if (serviceDuration <= TimeSpan.Zero)
        {
            return new ApiResponse<List<AvailableSlotResponse>>
            {
                Success = false,
                Message = "Service duration is invalid.",
                Data = []
            };
        }

        var workDayStart = date.Add(workingHour.OpenTime);
        var workDayEnd = date.Add(workingHour.CloseTime);

        var minimumAllowedStartTime = DateTime.UtcNow.AddMinutes(_options.MinimumNoticeMinutes);

        var existingBookings = await _context.Bookings
            .AsNoTracking()
            .Where(x =>
                x.ServiceBranchId == request.ServiceBranchId &&
                BlockingBookingStatuses.Contains(x.Status) &&
                x.StartDate < workDayEnd &&
                x.EndDate > workDayStart)
            .Select(x => new
            {
                x.StartDate,
                x.EndDate
            })
            .ToListAsync(cancellationToken);

        var availableSlots = new List<AvailableSlotResponse>();

        var slotStep = TimeSpan.FromMinutes(_options.SlotStepMinutes);
        var slotStart = workDayStart;

        while (slotStart.Add(serviceDuration) <= workDayEnd)
        {
            var slotEnd = slotStart.Add(serviceDuration);

            var isAfterMinimumNotice = slotStart >= minimumAllowedStartTime;

            var hasConflict = existingBookings.Any(existing =>
                slotStart < existing.EndDate &&
                slotEnd > existing.StartDate);

            if (isAfterMinimumNotice && !hasConflict)
            {
                availableSlots.Add(new AvailableSlotResponse
                {
                    StartTime = slotStart,
                    EndTime = slotEnd
                });
            }

            slotStart = slotStart.Add(slotStep);
        }

        return new ApiResponse<List<AvailableSlotResponse>>
        {
            Success = true,
            Message = "Available slots retrieved successfully.",
            Data = availableSlots
        };
    }

    public async Task<bool> IsSlotAvailableAsync(
        int serviceBranchId,
        DateTime startDate,
        DateTime endDate,
        ServiceLocationType locationType,
        int? excludedBookingId = null,
        CancellationToken cancellationToken = default)
    {
        if (serviceBranchId <= 0)
        {
            return false;
        }

        if (startDate >= endDate)
        {
            return false;
        }

        if (startDate < DateTime.UtcNow)
        {
            return false;
        }

        var workingHour = await _context.BranchWorkingHours
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.ServiceBranchId == serviceBranchId &&
                x.DayOfWeek == startDate.DayOfWeek,
                cancellationToken);

        if (workingHour is null || workingHour.IsClosed)
        {
            return false;
        }

        var workDayStart = startDate.Date.Add(workingHour.OpenTime);
        var workDayEnd = startDate.Date.Add(workingHour.CloseTime);

        if (startDate < workDayStart || endDate > workDayEnd)
        {
            return false;
        }

        var query = _context.Bookings
            .AsNoTracking()
            .Where(x =>
                x.ServiceBranchId == serviceBranchId &&
                BlockingBookingStatuses.Contains(x.Status) &&
                startDate < x.EndDate &&
                endDate > x.StartDate);

        if (excludedBookingId.HasValue)
        {
            query = query.Where(x => x.Id != excludedBookingId.Value);
        }

        var hasConflict = await query.AnyAsync(cancellationToken);

        return !hasConflict;
    }
}