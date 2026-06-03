using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Schedules;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class TechnicianScheduleService : ITechnicianScheduleService
{
    private readonly ApplicationDbContext _context;

    public TechnicianScheduleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<TechnicianScheduleResponse>> GetTechnicianScheduleAsync(
        int technicianId,
        TechnicianScheduleQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var fromDate = request.FromDate.Date;
        var toDate = request.ToDate.Date.AddDays(1).AddTicks(-1);

        var technician = await _context.Technicians
            .AsNoTracking()
            .Include(x => x.ServiceBranch)
            .Include(x => x.WorkingHours)
            .Include(x => x.UnavailableDates)
            .FirstOrDefaultAsync(x => x.Id == technicianId, cancellationToken);

        if (technician is null)
        {
            return ApiResponse<TechnicianScheduleResponse>.Fail("Technician not found.");
        }

        var bookingsQuery = _context.Bookings
            .AsNoTracking()
            .Include(x => x.Service)
            .Include(x => x.ServiceBranch)
            .Include(x => x.Technician)
            .Include(x => x.Car)
                .ThenInclude(x => x.CarTrim)
                    .ThenInclude(x => x.Year)
                        .ThenInclude(x => x.Model)
                            .ThenInclude(x => x.Brand)
            .Where(x =>
                x.TechnicianId == technicianId &&
                x.StartDate <= toDate &&
                x.EndDate >= fromDate);

        if (!request.IncludeCancelled)
        {
            bookingsQuery = bookingsQuery.Where(x => x.Status != BookingStatus.Cancelled);
        }

        var bookings = await bookingsQuery
            .OrderBy(x => x.StartDate)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        var customerLookup = await GetCustomerLookupAsync(bookings, cancellationToken);

        var unavailableDates = technician.UnavailableDates
            .Where(x =>
                x.IsActive &&
                x.StartDate <= DateOnly.FromDateTime(toDate) &&
                x.EndDate >= DateOnly.FromDateTime(fromDate))
            .OrderBy(x => x.StartDate)
            .ThenBy(x => x.StartTime)
            .Select(x => new TechnicianUnavailableScheduleResponse
            {
                Id = x.Id,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Reason = x.Reason
            })
            .ToList();

        var response = new TechnicianScheduleResponse
        {
            TechnicianId = technician.Id,
            TechnicianName = technician.FullName,
            ServiceBranchId = technician.ServiceBranchId,
            ServiceBranchName = technician.ServiceBranch.Name,
            FromDate = fromDate,
            ToDate = request.ToDate.Date,

            Summary = CreateSummary(bookings),

            WorkingHours = technician.WorkingHours
                .OrderBy(x => x.DayOfWeek)
                .Select(x => new TechnicianWorkingHourScheduleResponse
                {
                    DayOfWeek = x.DayOfWeek,
                    OpenTime = x.OpenTime,
                    CloseTime = x.CloseTime,
                    IsClosed = x.IsClosed
                })
                .ToList(),

            UnavailableDates = unavailableDates,

            Bookings = bookings
                .Select(x => MapBooking(x, customerLookup))
                .ToList()
        };

        return ApiResponse<TechnicianScheduleResponse>.Ok(response);
    }

    public async Task<ApiResponse<BranchScheduleResponse>> GetBranchScheduleAsync(
        int serviceBranchId,
        BranchScheduleQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var dayStart = request.Date.Date;
        var dayEnd = request.Date.Date.AddDays(1).AddTicks(-1);
        var day = request.Date.DayOfWeek;

        var branch = await _context.ServiceBranches
            .AsNoTracking()
            .Include(x => x.Technicians)
                .ThenInclude(x => x.WorkingHours)
            .Include(x => x.Technicians)
                .ThenInclude(x => x.UnavailableDates)
            .FirstOrDefaultAsync(x => x.Id == serviceBranchId, cancellationToken);

        if (branch is null)
        {
            return ApiResponse<BranchScheduleResponse>.Fail("Service branch not found.");
        }

        var bookingsQuery = _context.Bookings
            .AsNoTracking()
            .Include(x => x.Service)
            .Include(x => x.ServiceBranch)
            .Include(x => x.Technician)
            .Include(x => x.Car)
                .ThenInclude(x => x.CarTrim)
                    .ThenInclude(x => x.Year)
                        .ThenInclude(x => x.Model)
                            .ThenInclude(x => x.Brand)
            .Where(x =>
                x.ServiceBranchId == serviceBranchId &&
                x.StartDate <= dayEnd &&
                x.EndDate >= dayStart);

        if (!request.IncludeCancelled)
        {
            bookingsQuery = bookingsQuery.Where(x => x.Status != BookingStatus.Cancelled);
        }

        var bookings = await bookingsQuery
            .OrderBy(x => x.StartDate)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        var customerLookup = await GetCustomerLookupAsync(bookings, cancellationToken);

        var technicianSchedules = branch.Technicians
            .OrderBy(x => x.FullName)
            .ThenBy(x => x.Id)
            .Select(technician =>
            {
                var technicianBookings = bookings
                    .Where(b => b.TechnicianId == technician.Id)
                    .OrderBy(b => b.StartDate)
                    .ThenBy(b => b.Id)
                    .ToList();

                var unavailableDates = technician.UnavailableDates
                    .Where(x =>
                        x.IsActive &&
                        x.StartDate <= DateOnly.FromDateTime(dayEnd) &&
                        x.EndDate >= DateOnly.FromDateTime(dayStart))
                    .OrderBy(x => x.StartDate)
                    .ThenBy(x => x.StartTime)
                    .Select(x => new TechnicianUnavailableScheduleResponse
                    {
                        Id = x.Id,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime,
                        Reason = x.Reason
                    })
                    .ToList();

                return new BranchTechnicianScheduleResponse
                {
                    TechnicianId = technician.Id,
                    TechnicianName = technician.FullName,
                    IsActive = technician.IsActive,

                    WorkingHours = technician.WorkingHours
                        .Where(x => x.DayOfWeek == day)
                        .OrderBy(x => x.DayOfWeek)
                        .Select(x => new TechnicianWorkingHourScheduleResponse
                        {
                            DayOfWeek = x.DayOfWeek,
                            OpenTime = x.OpenTime,
                            CloseTime = x.CloseTime,
                            IsClosed = x.IsClosed
                        })
                        .ToList(),

                    UnavailableDates = unavailableDates,

                    Bookings = technicianBookings
                        .Select(x => MapBooking(x, customerLookup))
                        .ToList()
                };
            })
            .ToList();

        var unassignedBookings = bookings
            .Where(x => x.TechnicianId == null)
            .OrderBy(x => x.StartDate)
            .ThenBy(x => x.Id)
            .Select(x => MapBooking(x, customerLookup))
            .ToList();

        var response = new BranchScheduleResponse
        {
            ServiceBranchId = branch.Id,
            ServiceBranchName = branch.Name,
            Date = request.Date.Date,

            TotalBookings = bookings.Count,
            AssignedBookings = bookings.Count(x => x.TechnicianId.HasValue),
            UnassignedBookings = bookings.Count(x => x.TechnicianId == null),

            Technicians = technicianSchedules,
            UnassignedBookingsList = unassignedBookings
        };

        return ApiResponse<BranchScheduleResponse>.Ok(response);
    }

    private static TechnicianScheduleSummaryResponse CreateSummary(
        IReadOnlyCollection<Booking> bookings)
    {
        return new TechnicianScheduleSummaryResponse
        {
            TotalBookings = bookings.Count,
            PendingBookings = bookings.Count(x => x.Status == BookingStatus.Pending),
            ConfirmedBookings = bookings.Count(x => x.Status == BookingStatus.Confirmed),
            AssignedBookings = bookings.Count(x => x.Status == BookingStatus.Assigned),
            InProgressBookings = bookings.Count(x => x.Status == BookingStatus.InProgress),
            CompletedBookings = bookings.Count(x => x.Status == BookingStatus.Completed),
            CancelledBookings = bookings.Count(x => x.Status == BookingStatus.Cancelled)
        };
    }

    private static ScheduleBookingResponse MapBooking(
    Booking booking,
    IReadOnlyDictionary<string, CustomerLookupItem> users)
    {
        var carTrim = booking.Car.CarTrim;
        var carYear = carTrim.Year;
        var carModel = carYear.Model;
        var carBrand = carModel.Brand;

        users.TryGetValue(booking.UserId, out var customer);

        return new ScheduleBookingResponse
        {
            BookingId = booking.Id,

            ServiceId = booking.ServiceId,
            ServiceName = booking.Service.Name,

            CarId = booking.CarId,
            PlateNumber = booking.Car.PlateNumber,
            CarDisplayName = $"{carBrand.Name} {carModel.Name} {carYear.Year} {carTrim.Name}",

            CustomerName = customer?.Name,
            CustomerEmail = customer?.Email,

            TechnicianId = booking.TechnicianId,
            TechnicianName = booking.Technician?.FullName,

            ServiceBranchId = booking.ServiceBranchId,
            ServiceBranchName = booking.ServiceBranch?.Name,

            StartDate = booking.StartDate,
            EndDate = booking.EndDate,

            Status = booking.Status.ToString(),
            LocationType = booking.LocationType.ToString(),

            TotalPrice = booking.TotalPrice
        };
    }

    private async Task<Dictionary<string, CustomerLookupItem>> GetCustomerLookupAsync(
    IEnumerable<Booking> bookings,
    CancellationToken cancellationToken)
    {
        var userIds = bookings
            .Select(x => x.UserId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        if (userIds.Count == 0)
        {
            return new Dictionary<string, CustomerLookupItem>();
        }

        return await _context.Users
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .Select(x => new CustomerLookupItem
            {
                Id = x.Id,
                Name = x.FullName,
                Email = x.Email
            })
            .ToDictionaryAsync(x => x.Id, cancellationToken);
    }

    private sealed class CustomerLookupItem
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}