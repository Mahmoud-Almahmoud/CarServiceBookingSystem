using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class BookingAssignmentService : IBookingAssignmentService
{
    private readonly ApplicationDbContext _context;

    public BookingAssignmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<BookingTechnicianAssignmentResponse>> AssignTechnicianAsync(
        int bookingId,
        AssignTechnicianRequest request,
        CancellationToken cancellationToken = default)
    {
        var booking = await _context.Bookings
            .Include(x => x.Service)
            .Include(x => x.ServiceBranch)
            .FirstOrDefaultAsync(x => x.Id == bookingId, cancellationToken);

        if (booking is null)
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("Booking not found.");
        }

        if (booking.ServiceBranchId is null)
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("Booking does not have a selected service branch.");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("Cancelled bookings cannot be assigned to technicians.");
        }

        if (booking.Status == BookingStatus.Completed)
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("Completed bookings cannot be assigned to technicians.");
        }

        var technician = await _context.Technicians
            .AsNoTracking()
            .Include(x => x.ServiceBranch)
            .Include(x => x.TechnicianServices)
            .Include(x => x.WorkingHours)
            .Include(x => x.UnavailableDates)
            .FirstOrDefaultAsync(x => x.Id == request.TechnicianId, cancellationToken);

        if (technician is null)
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("Technician not found.");
        }

        if (!technician.IsActive)
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("Technician is inactive.");
        }

        if (technician.ServiceBranchId != booking.ServiceBranchId.Value)
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("Technician does not belong to the booking branch.");
        }

        var supportsService = technician.TechnicianServices
            .Any(x => x.ServiceId == booking.ServiceId);

        if (!supportsService)
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("Technician does not support this service.");
        }

        var isWorking = IsTechnicianWorking(
            technician.WorkingHours,
            booking.StartDate,
            booking.EndDate);

        if (!isWorking)
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("Technician is not working during the booking slot.");
        }

        var isUnavailable = IsTechnicianUnavailable(
            technician.UnavailableDates,
            booking.StartDate,
            booking.EndDate);

        if (isUnavailable)
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("Technician is unavailable during the booking slot.");
        }

        var hasOverlappingBooking = await _context.Bookings
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id != booking.Id &&
                x.TechnicianId == technician.Id &&
                x.Status != BookingStatus.Cancelled &&
                x.Status != BookingStatus.Completed &&
                x.StartDate < booking.EndDate &&
                x.EndDate > booking.StartDate,
                cancellationToken);

        if (hasOverlappingBooking)
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("Technician already has an overlapping booking.");
        }

        booking.TechnicianId = technician.Id;
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var response = new BookingTechnicianAssignmentResponse
        {
            BookingId = booking.Id,
            TechnicianId = technician.Id,
            TechnicianName = technician.FullName,
            ServiceBranchId = booking.ServiceBranchId.Value,
            ServiceBranchName = booking.ServiceBranch?.Name ?? technician.ServiceBranch.Name,
            ServiceId = booking.ServiceId,
            ServiceName = booking.Service?.Name ?? string.Empty,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            BookingStatus = booking.Status.ToString()
        };

        return ApiResponse<BookingTechnicianAssignmentResponse>.Ok(response);
    }

    public async Task<ApiResponse<BookingTechnicianAssignmentResponse>> AutoAssignTechnicianAsync(
    int bookingId,
    CancellationToken cancellationToken = default)
    {
        var booking = await _context.Bookings
            .Include(x => x.Service)
            .Include(x => x.ServiceBranch)
            .Include(x => x.Technician)
            .FirstOrDefaultAsync(x => x.Id == bookingId, cancellationToken);

        if (booking is null)
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("Booking not found.");
        }

        if (booking.ServiceBranchId is null)
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("Booking does not have a selected service branch.");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("Cancelled bookings cannot be assigned to technicians.");
        }

        if (booking.Status == BookingStatus.Completed)
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("Completed bookings cannot be assigned to technicians.");
        }

        if (booking.TechnicianId.HasValue)
        {
            var assignedTechnician = await _context.Technicians
                .AsNoTracking()
                .Include(x => x.ServiceBranch)
                .FirstOrDefaultAsync(x => x.Id == booking.TechnicianId.Value, cancellationToken);

            if (assignedTechnician is null)
            {
                return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("Booking has an assigned technician, but the technician was not found.");
            }

            return ApiResponse<BookingTechnicianAssignmentResponse>.Ok(
                CreateAssignmentResponse(booking, assignedTechnician),
                "Booking already has an assigned technician.");
        }

        var candidates = await _context.Technicians
            .AsNoTracking()
            .Include(x => x.ServiceBranch)
            .Include(x => x.TechnicianServices)
            .Include(x => x.WorkingHours)
            .Include(x => x.UnavailableDates)
            .Where(x =>
                x.IsActive &&
                x.ServiceBranchId == booking.ServiceBranchId.Value &&
                x.TechnicianServices.Any(s => s.ServiceId == booking.ServiceId))
            .ToListAsync(cancellationToken);

        if (!candidates.Any())
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("No active technician supports this service at the selected branch.");
        }

        var availableTechnicians = new List<TechnicianWorkloadCandidate>();

        foreach (var technician in candidates)
        {
            var isWorking = IsTechnicianWorking(
                technician.WorkingHours,
                booking.StartDate,
                booking.EndDate);

            if (!isWorking)
            {
                continue;
            }

            var isUnavailable = IsTechnicianUnavailable(
                technician.UnavailableDates,
                booking.StartDate,
                booking.EndDate);

            if (isUnavailable)
            {
                continue;
            }

            var hasOverlappingBooking = await _context.Bookings
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id != booking.Id &&
                    x.TechnicianId == technician.Id &&
                    x.Status != BookingStatus.Cancelled &&
                    x.Status != BookingStatus.Completed &&
                    x.StartDate < booking.EndDate &&
                    x.EndDate > booking.StartDate,
                    cancellationToken);

            if (hasOverlappingBooking)
            {
                continue;
            }

            var sameDayWorkload = await _context.Bookings
                .AsNoTracking()
                .CountAsync(x =>
                    x.TechnicianId == technician.Id &&
                    x.Status != BookingStatus.Cancelled &&
                    x.Status != BookingStatus.Completed &&
                    x.StartDate.Date == booking.StartDate.Date,
                    cancellationToken);

            availableTechnicians.Add(new TechnicianWorkloadCandidate
            {
                Technician = technician,
                SameDayWorkload = sameDayWorkload
            });
        }

        var selectedCandidate = availableTechnicians
            .OrderBy(x => x.SameDayWorkload)
            .ThenBy(x => x.Technician.Id)
            .FirstOrDefault();

        if (selectedCandidate is null)
        {
            return ApiResponse<BookingTechnicianAssignmentResponse>.Fail("No available technician found for this booking slot.");
        }

        booking.TechnicianId = selectedCandidate.Technician.Id;
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var response = CreateAssignmentResponse(booking, selectedCandidate.Technician);

        return ApiResponse<BookingTechnicianAssignmentResponse>.Ok(
            response,
            "Technician auto-assigned successfully.");
    }

    private static bool IsTechnicianWorking(
        IEnumerable<Domain.Entities.TechnicianWorkingHour> workingHours,
        DateTime slotStart,
        DateTime slotEnd)
    {
        if (slotStart.Date != slotEnd.Date)
        {
            return false;
        }

        var slotDay = slotStart.DayOfWeek;
        var slotStartTime = slotStart.TimeOfDay;
        var slotEndTime = slotEnd.TimeOfDay;

        var workingHour = workingHours.FirstOrDefault(x => x.DayOfWeek == slotDay);

        if (workingHour is null || workingHour.IsClosed)
        {
            return false;
        }

        return workingHour.OpenTime <= slotStartTime &&
               workingHour.CloseTime >= slotEndTime;
    }

    private static bool IsTechnicianUnavailable(
        IEnumerable<Domain.Entities.TechnicianUnavailableDate> unavailableDates,
        DateTime slotStart,
        DateTime slotEnd)
    {
        var slotStartDate = DateOnly.FromDateTime(slotStart);
        var slotEndDate = DateOnly.FromDateTime(slotEnd);

        var slotStartTime = slotStart.TimeOfDay;
        var slotEndTime = slotEnd.TimeOfDay;

        return unavailableDates
            .Where(x => x.IsActive)
            .Any(x =>
            {
                var dateOverlaps =
                    x.StartDate <= slotEndDate &&
                    x.EndDate >= slotStartDate;

                if (!dateOverlaps)
                {
                    return false;
                }

                var isFullDayUnavailable =
                    x.StartTime is null &&
                    x.EndTime is null;

                if (isFullDayUnavailable)
                {
                    return true;
                }

                if (x.StartTime is null || x.EndTime is null)
                {
                    return true;
                }

                return x.StartTime < slotEndTime &&
                       x.EndTime > slotStartTime;
            });
    }

    private static BookingTechnicianAssignmentResponse CreateAssignmentResponse(
    Domain.Entities.Booking booking,
    Domain.Entities.Technician technician)
    {
        return new BookingTechnicianAssignmentResponse
        {
            BookingId = booking.Id,
            TechnicianId = technician.Id,
            TechnicianName = technician.FullName,
            ServiceBranchId = booking.ServiceBranchId!.Value,
            ServiceBranchName = booking.ServiceBranch?.Name ?? technician.ServiceBranch?.Name ?? string.Empty,
            ServiceId = booking.ServiceId,
            ServiceName = booking.Service?.Name ?? string.Empty,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            BookingStatus = booking.Status.ToString()
        };
    }

    private sealed class TechnicianWorkloadCandidate
    {
        public Domain.Entities.Technician Technician { get; set; } = null!;
        public int SameDayWorkload { get; set; }
    }
}