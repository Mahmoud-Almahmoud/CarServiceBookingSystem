using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Technicians;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class TechnicianManagementService : ITechnicianService
{
    private readonly ApplicationDbContext _context;

    public TechnicianManagementService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PagedResponse<TechnicianResponse>>> GetAllAsync(
        TechnicianQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _context.Technicians
            .AsNoTracking()
            .Include(x => x.ServiceBranch)
            .Include(x => x.TechnicianServices)
                .ThenInclude(x => x.Service)
            .AsQueryable();

        if (request.ServiceBranchId.HasValue)
        {
            query = query.Where(x => x.ServiceBranchId == request.ServiceBranchId.Value);
        }

        if (request.ServiceId.HasValue)
        {
            query = query.Where(x => x.TechnicianServices.Any(s => s.ServiceId == request.ServiceId.Value));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.FullName.Contains(search) ||
                (x.PhoneNumber != null && x.PhoneNumber.Contains(search)) ||
                (x.Email != null && x.Email.Contains(search)));
        }

        query = request.SortBy?.ToLower() switch
        {
            "fullname" => request.Desc
                ? query.OrderByDescending(x => x.FullName).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.FullName).ThenBy(x => x.Id),

            "createdat" => request.Desc
                ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.Id)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new TechnicianResponse
            {
                Id = x.Id,
                ServiceBranchId = x.ServiceBranchId,
                ServiceBranchName = x.ServiceBranch.Name,
                FullName = x.FullName,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Services = x.TechnicianServices
                    .Select(s => new TechnicianServiceResponse
                    {
                        ServiceId = s.ServiceId,
                        ServiceName = s.Service.Name
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        var response = new PagedResponse<TechnicianResponse>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return ApiResponse<PagedResponse<TechnicianResponse>>.Ok(response);
    }

    public async Task<ApiResponse<TechnicianResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var technician = await _context.Technicians
            .AsNoTracking()
            .Include(x => x.ServiceBranch)
            .Include(x => x.TechnicianServices)
                .ThenInclude(x => x.Service)
            .Where(x => x.Id == id)
            .Select(x => new TechnicianResponse
            {
                Id = x.Id,
                ServiceBranchId = x.ServiceBranchId,
                ServiceBranchName = x.ServiceBranch.Name,
                FullName = x.FullName,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Services = x.TechnicianServices
                    .Select(s => new TechnicianServiceResponse
                    {
                        ServiceId = s.ServiceId,
                        ServiceName = s.Service.Name
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (technician is null)
        {
            return ApiResponse<TechnicianResponse>.Fail("Technician not found.");
        }

        return ApiResponse<TechnicianResponse>.Ok(technician);
    }

    public async Task<ApiResponse<TechnicianResponse>> CreateAsync(
        CreateTechnicianRequest request,
        CancellationToken cancellationToken = default)
    {
        var branchExists = await _context.ServiceBranches
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.ServiceBranchId, cancellationToken);

        if (!branchExists)
        {
            return ApiResponse<TechnicianResponse>.Fail("Service branch not found.");
        }

        var technician = new Technician
        {
            ServiceBranchId = request.ServiceBranchId,
            FullName = request.FullName.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.Technicians.Add(technician);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(technician.Id, cancellationToken);
    }

    public async Task<ApiResponse<TechnicianResponse>> UpdateAsync(
        int id,
        UpdateTechnicianRequest request,
        CancellationToken cancellationToken = default)
    {
        var technician = await _context.Technicians
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (technician is null)
        {
            return ApiResponse<TechnicianResponse>.Fail("Technician not found.");
        }

        var branchExists = await _context.ServiceBranches
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.ServiceBranchId, cancellationToken);

        if (!branchExists)
        {
            return ApiResponse<TechnicianResponse>.Fail("Service branch not found.");
        }

        technician.ServiceBranchId = request.ServiceBranchId;
        technician.FullName = request.FullName.Trim();
        technician.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
        technician.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        technician.IsActive = request.IsActive;
        technician.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(technician.Id, cancellationToken);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var technician = await _context.Technicians
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (technician is null)
        {
            return ApiResponse<bool>.Fail("Technician not found.");
        }

        var hasBookings = await _context.Bookings
            .AsNoTracking()
            .AnyAsync(x => x.TechnicianId == id, cancellationToken);

        if (hasBookings)
        {
            technician.IsActive = false;
            technician.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Technician has bookings, so it was deactivated instead of deleted.");
        }

        _context.Technicians.Remove(technician);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }

    public async Task<ApiResponse<IEnumerable<TechnicianServiceResponse>>> GetServicesAsync(
        int technicianId,
        CancellationToken cancellationToken = default)
    {
        var technicianExists = await _context.Technicians
            .AsNoTracking()
            .AnyAsync(x => x.Id == technicianId, cancellationToken);

        if (!technicianExists)
        {
            return ApiResponse<IEnumerable<TechnicianServiceResponse>>.Fail("Technician not found.");
        }

        var services = await _context.TechnicianServices
            .AsNoTracking()
            .Include(x => x.Service)
            .Where(x => x.TechnicianId == technicianId)
            .Select(x => new TechnicianServiceResponse
            {
                ServiceId = x.ServiceId,
                ServiceName = x.Service.Name
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<TechnicianServiceResponse>>.Ok(services);
    }

    public async Task<ApiResponse<TechnicianServiceResponse>> AddServiceAsync(
        int technicianId,
        AddTechnicianServiceRequest request,
        CancellationToken cancellationToken = default)
    {
        var technician = await _context.Technicians
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == technicianId, cancellationToken);

        if (technician is null)
        {
            return ApiResponse<TechnicianServiceResponse>.Fail("Technician not found.");
        }

        var service = await _context.Services
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.ServiceId, cancellationToken);

        if (service is null)
        {
            return ApiResponse<TechnicianServiceResponse>.Fail("Service not found.");
        }

        var alreadyExists = await _context.TechnicianServices
            .AsNoTracking()
            .AnyAsync(x =>
                x.TechnicianId == technicianId &&
                x.ServiceId == request.ServiceId,
                cancellationToken);

        if (alreadyExists)
        {
            return ApiResponse<TechnicianServiceResponse>.Fail("Technician already supports this service.");
        }

        var technicianService = new Domain.Entities.TechnicianService
        {
            TechnicianId = technicianId,
            ServiceId = request.ServiceId,
            CreatedAt = DateTime.UtcNow
        };

        _context.TechnicianServices.Add(technicianService);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new TechnicianServiceResponse
        {
            ServiceId = service.Id,
            ServiceName = service.Name
        };

        return ApiResponse<TechnicianServiceResponse>.Ok(response);
    }

    public async Task<ApiResponse<bool>> RemoveServiceAsync(
        int technicianId,
        int serviceId,
        CancellationToken cancellationToken = default)
    {
        var technicianService = await _context.TechnicianServices
            .FirstOrDefaultAsync(x =>
                x.TechnicianId == technicianId &&
                x.ServiceId == serviceId,
                cancellationToken);

        if (technicianService is null)
        {
            return ApiResponse<bool>.Fail("Technician service not found.");
        }

        _context.TechnicianServices.Remove(technicianService);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }

    public async Task<ApiResponse<IEnumerable<TechnicianWorkingHourResponse>>> GetWorkingHoursAsync(
    int technicianId,
    CancellationToken cancellationToken = default)
    {
        var technicianExists = await _context.Technicians
            .AsNoTracking()
            .AnyAsync(x => x.Id == technicianId, cancellationToken);

        if (!technicianExists)
        {
            return ApiResponse<IEnumerable<TechnicianWorkingHourResponse>>.Fail("Technician not found.");
        }

        var workingHours = await _context.TechnicianWorkingHours
            .AsNoTracking()
            .Where(x => x.TechnicianId == technicianId)
            .OrderBy(x => x.DayOfWeek)
            .Select(x => new TechnicianWorkingHourResponse
            {
                Id = x.Id,
                TechnicianId = x.TechnicianId,
                DayOfWeek = x.DayOfWeek,
                OpenTime = x.OpenTime,
                CloseTime = x.CloseTime,
                IsClosed = x.IsClosed
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<TechnicianWorkingHourResponse>>.Ok(workingHours);
    }

    public async Task<ApiResponse<IEnumerable<TechnicianWorkingHourResponse>>> UpdateWorkingHoursAsync(
        int technicianId,
        UpdateTechnicianWorkingHoursRequest request,
        CancellationToken cancellationToken = default)
    {
        var technicianExists = await _context.Technicians
            .AsNoTracking()
            .AnyAsync(x => x.Id == technicianId, cancellationToken);

        if (!technicianExists)
        {
            return ApiResponse<IEnumerable<TechnicianWorkingHourResponse>>.Fail("Technician not found.");
        }

        var existingWorkingHours = await _context.TechnicianWorkingHours
            .Where(x => x.TechnicianId == technicianId)
            .ToListAsync(cancellationToken);

        _context.TechnicianWorkingHours.RemoveRange(existingWorkingHours);

        var newWorkingHours = request.WorkingHours
            .Select(x => new TechnicianWorkingHour
            {
                TechnicianId = technicianId,
                DayOfWeek = x.DayOfWeek,
                OpenTime = x.OpenTime,
                CloseTime = x.CloseTime,
                IsClosed = x.IsClosed,
                CreatedAt = DateTime.UtcNow
            })
            .ToList();

        _context.TechnicianWorkingHours.AddRange(newWorkingHours);

        await _context.SaveChangesAsync(cancellationToken);

        return await GetWorkingHoursAsync(technicianId, cancellationToken);
    }

    public async Task<ApiResponse<IEnumerable<TechnicianUnavailableDateResponse>>> GetUnavailableDatesAsync(
    int technicianId,
    CancellationToken cancellationToken = default)
    {
        var technicianExists = await _context.Technicians
            .AsNoTracking()
            .AnyAsync(x => x.Id == technicianId, cancellationToken);

        if (!technicianExists)
        {
            return ApiResponse<IEnumerable<TechnicianUnavailableDateResponse>>.Fail("Technician not found.");
        }

        var unavailableDates = await _context.TechnicianUnavailableDates
            .AsNoTracking()
            .Where(x => x.TechnicianId == technicianId)
            .OrderByDescending(x => x.StartDate)
            .ThenByDescending(x => x.Id)
            .Select(x => new TechnicianUnavailableDateResponse
            {
                Id = x.Id,
                TechnicianId = x.TechnicianId,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Reason = x.Reason,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<TechnicianUnavailableDateResponse>>.Ok(unavailableDates);
    }

    public async Task<ApiResponse<TechnicianUnavailableDateResponse>> GetUnavailableDateByIdAsync(
        int technicianId,
        int unavailableDateId,
        CancellationToken cancellationToken = default)
    {
        var unavailableDate = await _context.TechnicianUnavailableDates
            .AsNoTracking()
            .Where(x => x.TechnicianId == technicianId && x.Id == unavailableDateId)
            .Select(x => new TechnicianUnavailableDateResponse
            {
                Id = x.Id,
                TechnicianId = x.TechnicianId,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Reason = x.Reason,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (unavailableDate is null)
        {
            return ApiResponse<TechnicianUnavailableDateResponse>.Fail("Technician unavailable date not found.");
        }

        return ApiResponse<TechnicianUnavailableDateResponse>.Ok(unavailableDate);
    }

    public async Task<ApiResponse<TechnicianUnavailableDateResponse>> CreateUnavailableDateAsync(
        int technicianId,
        CreateTechnicianUnavailableDateRequest request,
        CancellationToken cancellationToken = default)
    {
        var technicianExists = await _context.Technicians
            .AsNoTracking()
            .AnyAsync(x => x.Id == technicianId, cancellationToken);

        if (!technicianExists)
        {
            return ApiResponse<TechnicianUnavailableDateResponse>.Fail("Technician not found.");
        }

        var unavailableDate = new TechnicianUnavailableDate
        {
            TechnicianId = technicianId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Reason = string.IsNullOrWhiteSpace(request.Reason) ? null : request.Reason.Trim(),
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.TechnicianUnavailableDates.Add(unavailableDate);

        await _context.SaveChangesAsync(cancellationToken);

        return await GetUnavailableDateByIdAsync(technicianId, unavailableDate.Id, cancellationToken);
    }

    public async Task<ApiResponse<TechnicianUnavailableDateResponse>> UpdateUnavailableDateAsync(
        int technicianId,
        int unavailableDateId,
        UpdateTechnicianUnavailableDateRequest request,
        CancellationToken cancellationToken = default)
    {
        var unavailableDate = await _context.TechnicianUnavailableDates
            .FirstOrDefaultAsync(x =>
                x.TechnicianId == technicianId &&
                x.Id == unavailableDateId,
                cancellationToken);

        if (unavailableDate is null)
        {
            return ApiResponse<TechnicianUnavailableDateResponse>.Fail("Technician unavailable date not found.");
        }

        unavailableDate.StartDate = request.StartDate;
        unavailableDate.EndDate = request.EndDate;
        unavailableDate.StartTime = request.StartTime;
        unavailableDate.EndTime = request.EndTime;
        unavailableDate.Reason = string.IsNullOrWhiteSpace(request.Reason) ? null : request.Reason.Trim();
        unavailableDate.IsActive = request.IsActive;
        unavailableDate.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await GetUnavailableDateByIdAsync(technicianId, unavailableDate.Id, cancellationToken);
    }

    public async Task<ApiResponse<bool>> DeleteUnavailableDateAsync(
        int technicianId,
        int unavailableDateId,
        CancellationToken cancellationToken = default)
    {
        var unavailableDate = await _context.TechnicianUnavailableDates
            .FirstOrDefaultAsync(x =>
                x.TechnicianId == technicianId &&
                x.Id == unavailableDateId,
                cancellationToken);

        if (unavailableDate is null)
        {
            return ApiResponse<bool>.Fail("Technician unavailable date not found.");
        }

        _context.TechnicianUnavailableDates.Remove(unavailableDate);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }
}