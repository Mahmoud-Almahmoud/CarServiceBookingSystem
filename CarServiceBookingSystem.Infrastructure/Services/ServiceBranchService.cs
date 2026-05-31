using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.ServiceBranches;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class ServiceBranchService : IServiceBranchService
{
    private readonly ApplicationDbContext _context;

    public ServiceBranchService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PagedResponse<ServiceBranchResponse>>> GetBranchesAsync(
        ServiceBranchFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _context.ServiceBranches
            .AsNoTracking()
            .Include(x => x.BranchServices)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.CountryCode))
        {
            var countryCode = NormalizeCountryCode(request.CountryCode);
            query = query.Where(x => x.CountryCode == countryCode);
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var city = NormalizeCity(request.City);
            query = query.Where(x => x.City.ToLower() == city);
        }

        if (request.ServiceId.HasValue)
        {
            query = query.Where(x =>
                x.BranchServices.Any(bs =>
                    bs.ServiceId == request.ServiceId.Value &&
                    bs.IsActive));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();

            query = query.Where(x =>
                x.Name.ToLower().Contains(search) ||
                x.CountryCode.ToLower().Contains(search) ||
                x.City.ToLower().Contains(search));
        }

        query = request.SortBy?.ToLower() switch
        {
            "name" => request.Desc
                ? query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Name).ThenBy(x => x.Id),

            "city" => request.Desc
                ? query.OrderByDescending(x => x.City).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.City).ThenBy(x => x.Id),

            "country" => request.Desc
                ? query.OrderByDescending(x => x.CountryCode).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CountryCode).ThenBy(x => x.Id),

            "createdat" => request.Desc
                ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.Id)
        };

        var totalRecords = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ServiceBranchResponse
            {
                Id = x.Id,
                Name = x.Name,
                CountryCode = x.CountryCode,
                City = x.City,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                IsActive = x.IsActive,
                ActiveServicesCount = x.BranchServices.Count(bs => bs.IsActive),
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new ApiResponse<PagedResponse<ServiceBranchResponse>>
        {
            Success = true,
            Message = "Service branches retrieved successfully.",
            Data = new PagedResponse<ServiceBranchResponse>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecords
            }
        };
    }

    public async Task<ApiResponse<ServiceBranchResponse>> GetBranchByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var branch = await _context.ServiceBranches
            .AsNoTracking()
            .Include(x => x.BranchServices)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (branch is null)
        {
            return new ApiResponse<ServiceBranchResponse>
            {
                Success = false,
                Message = "Service branch was not found."
            };
        }

        return new ApiResponse<ServiceBranchResponse>
        {
            Success = true,
            Message = "Service branch retrieved successfully.",
            Data = ToResponse(branch)
        };
    }

    public async Task<ApiResponse<ServiceBranchResponse>> CreateBranchAsync(
        CreateServiceBranchRequest request,
        CancellationToken cancellationToken = default)
    {
        var countryCode = NormalizeCountryCode(request.CountryCode);
        var city = NormalizeCityForStorage(request.City);

        var duplicateExists = await _context.ServiceBranches
            .AnyAsync(x =>
                x.Name.ToLower() == request.Name.Trim().ToLower() &&
                x.CountryCode == countryCode &&
                x.City.ToLower() == city.ToLower(),
                cancellationToken);

        if (duplicateExists)
        {
            return new ApiResponse<ServiceBranchResponse>
            {
                Success = false,
                Message = "A service branch with the same name and location already exists."
            };
        }

        var branch = new ServiceBranch
        {
            Name = request.Name.Trim(),
            CountryCode = countryCode,
            City = city,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _context.ServiceBranches.AddAsync(branch, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new ApiResponse<ServiceBranchResponse>
        {
            Success = true,
            Message = "Service branch created successfully.",
            Data = ToResponse(branch)
        };
    }

    public async Task<ApiResponse<ServiceBranchResponse>> UpdateBranchAsync(
        int id,
        UpdateServiceBranchRequest request,
        CancellationToken cancellationToken = default)
    {
        var branch = await _context.ServiceBranches
            .Include(x => x.BranchServices)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (branch is null)
        {
            return new ApiResponse<ServiceBranchResponse>
            {
                Success = false,
                Message = "Service branch was not found."
            };
        }

        var countryCode = NormalizeCountryCode(request.CountryCode);
        var city = NormalizeCityForStorage(request.City);

        var duplicateExists = await _context.ServiceBranches
            .AnyAsync(x =>
                x.Id != id &&
                x.Name.ToLower() == request.Name.Trim().ToLower() &&
                x.CountryCode == countryCode &&
                x.City.ToLower() == city.ToLower(),
                cancellationToken);

        if (duplicateExists)
        {
            return new ApiResponse<ServiceBranchResponse>
            {
                Success = false,
                Message = "A service branch with the same name and location already exists."
            };
        }

        branch.Name = request.Name.Trim();
        branch.CountryCode = countryCode;
        branch.City = city;
        branch.Latitude = request.Latitude;
        branch.Longitude = request.Longitude;
        branch.IsActive = request.IsActive;
        branch.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new ApiResponse<ServiceBranchResponse>
        {
            Success = true,
            Message = "Service branch updated successfully.",
            Data = ToResponse(branch)
        };
    }

    public async Task<ApiResponse<string>> DeleteBranchAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var branch = await _context.ServiceBranches
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (branch is null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Service branch was not found."
            };
        }

        var usedByBookings = await _context.Bookings
            .AnyAsync(x => x.ServiceBranchId == id, cancellationToken);

        if (usedByBookings)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Cannot delete this branch because it is assigned to bookings. Deactivate it instead."
            };
        }

        _context.ServiceBranches.Remove(branch);
        await _context.SaveChangesAsync(cancellationToken);

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Service branch deleted successfully.",
            Data = "Deleted"
        };
    }

    public async Task<ApiResponse<List<BranchServiceResponse>>> GetBranchServicesAsync(
        int branchId,
        CancellationToken cancellationToken = default)
    {
        var branchExists = await _context.ServiceBranches
            .AnyAsync(x => x.Id == branchId, cancellationToken);

        if (!branchExists)
        {
            return new ApiResponse<List<BranchServiceResponse>>
            {
                Success = false,
                Message = "Service branch was not found."
            };
        }

        var services = await _context.BranchServices
            .AsNoTracking()
            .Include(x => x.ServiceBranch)
            .Include(x => x.Service)
            .Where(x => x.ServiceBranchId == branchId)
            .OrderByDescending(x => x.Id)
            .Select(x => new BranchServiceResponse
            {
                Id = x.Id,
                ServiceBranchId = x.ServiceBranchId,
                BranchName = x.ServiceBranch.Name,
                ServiceId = x.ServiceId,
                ServiceName = x.Service.Name,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new ApiResponse<List<BranchServiceResponse>>
        {
            Success = true,
            Message = "Branch services retrieved successfully.",
            Data = services
        };
    }

    public async Task<ApiResponse<BranchServiceResponse>> AddBranchServiceAsync(
        int branchId,
        AddBranchServiceRequest request,
        CancellationToken cancellationToken = default)
    {
        var branch = await _context.ServiceBranches
            .FirstOrDefaultAsync(x => x.Id == branchId, cancellationToken);

        if (branch is null)
        {
            return new ApiResponse<BranchServiceResponse>
            {
                Success = false,
                Message = "Service branch was not found."
            };
        }

        var serviceExists = await _context.Services
            .AnyAsync(x => x.Id == request.ServiceId, cancellationToken);

        if (!serviceExists)
        {
            return new ApiResponse<BranchServiceResponse>
            {
                Success = false,
                Message = "Service was not found."
            };
        }

        var existing = await _context.BranchServices
            .FirstOrDefaultAsync(x =>
                x.ServiceBranchId == branchId &&
                x.ServiceId == request.ServiceId,
                cancellationToken);

        if (existing is not null)
        {
            existing.IsActive = request.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            var updated = await _context.BranchServices
                .AsNoTracking()
                .Include(x => x.ServiceBranch)
                .Include(x => x.Service)
                .FirstAsync(x => x.Id == existing.Id, cancellationToken);

            return new ApiResponse<BranchServiceResponse>
            {
                Success = true,
                Message = "Branch service updated successfully.",
                Data = ToBranchServiceResponse(updated)
            };
        }

        var branchService = new BranchService
        {
            ServiceBranchId = branchId,
            ServiceId = request.ServiceId,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _context.BranchServices.AddAsync(branchService, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.BranchServices
            .AsNoTracking()
            .Include(x => x.ServiceBranch)
            .Include(x => x.Service)
            .FirstAsync(x => x.Id == branchService.Id, cancellationToken);

        return new ApiResponse<BranchServiceResponse>
        {
            Success = true,
            Message = "Service assigned to branch successfully.",
            Data = ToBranchServiceResponse(created)
        };
    }

    public async Task<ApiResponse<string>> RemoveBranchServiceAsync(
        int branchId,
        int serviceId,
        CancellationToken cancellationToken = default)
    {
        var branchService = await _context.BranchServices
            .FirstOrDefaultAsync(x =>
                x.ServiceBranchId == branchId &&
                x.ServiceId == serviceId,
                cancellationToken);

        if (branchService is null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Branch service was not found."
            };
        }

        _context.BranchServices.Remove(branchService);
        await _context.SaveChangesAsync(cancellationToken);

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Service removed from branch successfully.",
            Data = "Removed"
        };
    }

    public async Task<ApiResponse<ServiceBranchSelectionResponse>> SelectNearestBranchAsync(
        int serviceId,
        decimal customerLatitude,
        decimal customerLongitude,
        CancellationToken cancellationToken = default)
    {
        var branches = await _context.ServiceBranches
            .AsNoTracking()
            .Include(x => x.BranchServices)
            .Where(x =>
                x.IsActive &&
                x.BranchServices.Any(bs =>
                    bs.ServiceId == serviceId &&
                    bs.IsActive))
            .ToListAsync(cancellationToken);

        if (branches.Count == 0)
        {
            return new ApiResponse<ServiceBranchSelectionResponse>
            {
                Success = false,
                Message = "No active branch offers this service."
            };
        }

        var selectedBranch = branches
            .Select(branch => new
            {
                Branch = branch,
                DistanceKm = CalculateDistanceKm(
                    (double)branch.Latitude,
                    (double)branch.Longitude,
                    (double)customerLatitude,
                    (double)customerLongitude)
            })
            .OrderBy(x => x.DistanceKm)
            .ThenBy(x => x.Branch.Id)
            .First();

        return new ApiResponse<ServiceBranchSelectionResponse>
        {
            Success = true,
            Message = "Nearest service branch selected successfully.",
            Data = new ServiceBranchSelectionResponse
            {
                ServiceBranchId = selectedBranch.Branch.Id,
                ServiceBranchName = selectedBranch.Branch.Name,
                CountryCode = selectedBranch.Branch.CountryCode,
                City = selectedBranch.Branch.City,
                Latitude = selectedBranch.Branch.Latitude,
                Longitude = selectedBranch.Branch.Longitude,
                StraightLineDistanceKm = Math.Round(selectedBranch.DistanceKm, 2)
            }
        };
    }

    public async Task<ApiResponse<List<BranchWorkingHourResponse>>> GetWorkingHoursAsync(
    int branchId,
    CancellationToken cancellationToken = default)
    {
        var branchExists = await _context.ServiceBranches
            .AnyAsync(x => x.Id == branchId, cancellationToken);

        if (!branchExists)
        {
            return new ApiResponse<List<BranchWorkingHourResponse>>
            {
                Success = false,
                Message = "Service branch was not found."
            };
        }

        var workingHours = await _context.BranchWorkingHours
            .AsNoTracking()
            .Include(x => x.ServiceBranch)
            .Where(x => x.ServiceBranchId == branchId)
            .OrderBy(x => x.DayOfWeek)
            .Select(x => new BranchWorkingHourResponse
            {
                Id = x.Id,
                ServiceBranchId = x.ServiceBranchId,
                BranchName = x.ServiceBranch.Name,
                DayOfWeek = x.DayOfWeek,
                OpenTime = x.OpenTime,
                CloseTime = x.CloseTime,
                IsClosed = x.IsClosed
            })
            .ToListAsync(cancellationToken);

        return new ApiResponse<List<BranchWorkingHourResponse>>
        {
            Success = true,
            Message = "Branch working hours retrieved successfully.",
            Data = workingHours
        };
    }

    public async Task<ApiResponse<List<BranchWorkingHourResponse>>> UpdateWorkingHoursAsync(
    int branchId,
    UpdateBranchWorkingHoursRequest request,
    CancellationToken cancellationToken = default)
    {
        var branch = await _context.ServiceBranches
            .Include(x => x.WorkingHours)
            .FirstOrDefaultAsync(x => x.Id == branchId, cancellationToken);

        if (branch is null)
        {
            return new ApiResponse<List<BranchWorkingHourResponse>>
            {
                Success = false,
                Message = "Service branch was not found."
            };
        }

        var duplicateDays = request.WorkingHours
            .GroupBy(x => x.DayOfWeek)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        if (duplicateDays.Count > 0)
        {
            return new ApiResponse<List<BranchWorkingHourResponse>>
            {
                Success = false,
                Message = "Duplicate working-hour rows are not allowed for the same day."
            };
        }

        foreach (var item in request.WorkingHours)
        {
            if (!item.IsClosed && item.OpenTime >= item.CloseTime)
            {
                return new ApiResponse<List<BranchWorkingHourResponse>>
                {
                    Success = false,
                    Message = $"OpenTime must be before CloseTime for {item.DayOfWeek}."
                };
            }

            var existing = branch.WorkingHours
                .FirstOrDefault(x => x.DayOfWeek == item.DayOfWeek);

            if (existing is null)
            {
                branch.WorkingHours.Add(new BranchWorkingHour
                {
                    ServiceBranchId = branchId,
                    DayOfWeek = item.DayOfWeek,
                    OpenTime = item.OpenTime,
                    CloseTime = item.CloseTime,
                    IsClosed = item.IsClosed,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                existing.OpenTime = item.OpenTime;
                existing.CloseTime = item.CloseTime;
                existing.IsClosed = item.IsClosed;
                existing.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await GetWorkingHoursAsync(branchId, cancellationToken);
    }

    public async Task<ApiResponse<ServiceBranchSelectionResponse>> GetBranchForStoreBookingAsync(
    int branchId,
    int serviceId,
    CancellationToken cancellationToken = default)
    {
        var branch = await _context.ServiceBranches
            .AsNoTracking()
            .Include(x => x.BranchServices)
            .FirstOrDefaultAsync(x =>
                x.Id == branchId &&
                x.IsActive,
                cancellationToken);

        if (branch is null)
        {
            return new ApiResponse<ServiceBranchSelectionResponse>
            {
                Success = false,
                Message = "Service branch was not found."
            };
        }

        var offersService = branch.BranchServices.Any(x =>
            x.ServiceId == serviceId &&
            x.IsActive);

        if (!offersService)
        {
            return new ApiResponse<ServiceBranchSelectionResponse>
            {
                Success = false,
                Message = "This branch does not offer the selected service."
            };
        }

        return new ApiResponse<ServiceBranchSelectionResponse>
        {
            Success = true,
            Message = "Service branch selected successfully.",
            Data = new ServiceBranchSelectionResponse
            {
                ServiceBranchId = branch.Id,
                ServiceBranchName = branch.Name,
                CountryCode = branch.CountryCode,
                City = branch.City,
                Latitude = branch.Latitude,
                Longitude = branch.Longitude,
                StraightLineDistanceKm = 0
            }
        };
    }

    public async Task<ApiResponse<PagedResponse<BranchClosureResponse>>> GetClosuresAsync(
    int branchId,
    BranchClosureFilterRequest request,
    CancellationToken cancellationToken = default)
    {
        var branchExists = await _context.ServiceBranches
            .AnyAsync(x => x.Id == branchId, cancellationToken);

        if (!branchExists)
        {
            return new ApiResponse<PagedResponse<BranchClosureResponse>>
            {
                Success = false,
                Message = "Service branch was not found."
            };
        }

        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _context.BranchClosures
            .AsNoTracking()
            .Include(x => x.ServiceBranch)
            .Where(x => x.ServiceBranchId == branchId)
            .AsQueryable();

        if (request.FromDate.HasValue)
        {
            var fromDate = request.FromDate.Value.Date;
            query = query.Where(x => x.EndDate.Date >= fromDate);
        }

        if (request.ToDate.HasValue)
        {
            var toDate = request.ToDate.Value.Date;
            query = query.Where(x => x.StartDate.Date <= toDate);
        }

        if (request.Type.HasValue)
        {
            query = query.Where(x => x.Type == request.Type.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();

            query = query.Where(x =>
                x.Reason.ToLower().Contains(search) ||
                x.ServiceBranch.Name.ToLower().Contains(search));
        }

        query = request.SortBy?.ToLower() switch
        {
            "startdate" => request.Desc
                ? query.OrderByDescending(x => x.StartDate).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.StartDate).ThenBy(x => x.Id),

            "enddate" => request.Desc
                ? query.OrderByDescending(x => x.EndDate).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.EndDate).ThenBy(x => x.Id),

            "type" => request.Desc
                ? query.OrderByDescending(x => x.Type).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Type).ThenBy(x => x.Id),

            "createdat" => request.Desc
                ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.StartDate).ThenByDescending(x => x.Id)
        };

        var totalRecords = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new BranchClosureResponse
            {
                Id = x.Id,
                ServiceBranchId = x.ServiceBranchId,
                BranchName = x.ServiceBranch.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsFullDay = x.IsFullDay,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Type = x.Type,
                Reason = x.Reason,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new ApiResponse<PagedResponse<BranchClosureResponse>>
        {
            Success = true,
            Message = "Branch closures retrieved successfully.",
            Data = new PagedResponse<BranchClosureResponse>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecords
            }
        };
    }

    public async Task<ApiResponse<BranchClosureResponse>> GetClosureByIdAsync(
    int branchId,
    int closureId,
    CancellationToken cancellationToken = default)
    {
        var closure = await _context.BranchClosures
            .AsNoTracking()
            .Include(x => x.ServiceBranch)
            .FirstOrDefaultAsync(x =>
                x.Id == closureId &&
                x.ServiceBranchId == branchId,
                cancellationToken);

        if (closure is null)
        {
            return new ApiResponse<BranchClosureResponse>
            {
                Success = false,
                Message = "Branch closure was not found."
            };
        }

        return new ApiResponse<BranchClosureResponse>
        {
            Success = true,
            Message = "Branch closure retrieved successfully.",
            Data = ToBranchClosureResponse(closure)
        };
    }

    public async Task<ApiResponse<BranchClosureResponse>> CreateClosureAsync(
    int branchId,
    CreateBranchClosureRequest request,
    CancellationToken cancellationToken = default)
    {
        var branchExists = await _context.ServiceBranches
            .AnyAsync(x => x.Id == branchId, cancellationToken);

        if (!branchExists)
        {
            return new ApiResponse<BranchClosureResponse>
            {
                Success = false,
                Message = "Service branch was not found."
            };
        }

        var validationResult = ValidateClosureInput(
            request.StartDate,
            request.EndDate,
            request.IsFullDay,
            request.StartTime,
            request.EndTime);

        if (!validationResult.Success)
        {
            return new ApiResponse<BranchClosureResponse>
            {
                Success = false,
                Message = validationResult.Message
            };
        }

        var closure = new BranchClosure
        {
            ServiceBranchId = branchId,
            StartDate = request.StartDate.Date,
            EndDate = request.EndDate.Date,
            IsFullDay = request.IsFullDay,
            StartTime = request.IsFullDay ? null : request.StartTime,
            EndTime = request.IsFullDay ? null : request.EndTime,
            Type = request.Type,
            Reason = request.Reason.Trim(),
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _context.BranchClosures.AddAsync(closure, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.BranchClosures
            .AsNoTracking()
            .Include(x => x.ServiceBranch)
            .FirstAsync(x => x.Id == closure.Id, cancellationToken);

        return new ApiResponse<BranchClosureResponse>
        {
            Success = true,
            Message = "Branch closure created successfully.",
            Data = ToBranchClosureResponse(created)
        };
    }

    public async Task<ApiResponse<BranchClosureResponse>> UpdateClosureAsync(
    int branchId,
    int closureId,
    UpdateBranchClosureRequest request,
    CancellationToken cancellationToken = default)
    {
        var closure = await _context.BranchClosures
            .Include(x => x.ServiceBranch)
            .FirstOrDefaultAsync(x =>
                x.Id == closureId &&
                x.ServiceBranchId == branchId,
                cancellationToken);

        if (closure is null)
        {
            return new ApiResponse<BranchClosureResponse>
            {
                Success = false,
                Message = "Branch closure was not found."
            };
        }

        var validationResult = ValidateClosureInput(
            request.StartDate,
            request.EndDate,
            request.IsFullDay,
            request.StartTime,
            request.EndTime);

        if (!validationResult.Success)
        {
            return new ApiResponse<BranchClosureResponse>
            {
                Success = false,
                Message = validationResult.Message
            };
        }

        closure.StartDate = request.StartDate.Date;
        closure.EndDate = request.EndDate.Date;
        closure.IsFullDay = request.IsFullDay;
        closure.StartTime = request.IsFullDay ? null : request.StartTime;
        closure.EndTime = request.IsFullDay ? null : request.EndTime;
        closure.Type = request.Type;
        closure.Reason = request.Reason.Trim();
        closure.IsActive = request.IsActive;
        closure.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new ApiResponse<BranchClosureResponse>
        {
            Success = true,
            Message = "Branch closure updated successfully.",
            Data = ToBranchClosureResponse(closure)
        };
    }

    public async Task<ApiResponse<string>> DeleteClosureAsync(
    int branchId,
    int closureId,
    CancellationToken cancellationToken = default)
    {
        var closure = await _context.BranchClosures
            .FirstOrDefaultAsync(x =>
                x.Id == closureId &&
                x.ServiceBranchId == branchId,
                cancellationToken);

        if (closure is null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Branch closure was not found."
            };
        }

        _context.BranchClosures.Remove(closure);
        await _context.SaveChangesAsync(cancellationToken);

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Branch closure deleted successfully.",
            Data = "Deleted"
        };
    }

    public async Task<ApiResponse<PagedResponse<BranchCapacityRuleResponse>>> GetCapacityRulesAsync(
    int branchId,
    BranchCapacityRuleFilterRequest request,
    CancellationToken cancellationToken = default)
    {
        var branchExists = await _context.ServiceBranches
            .AnyAsync(x => x.Id == branchId, cancellationToken);

        if (!branchExists)
        {
            return new ApiResponse<PagedResponse<BranchCapacityRuleResponse>>
            {
                Success = false,
                Message = "Service branch was not found."
            };
        }

        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _context.BranchCapacityRules
            .AsNoTracking()
            .Include(x => x.ServiceBranch)
            .Where(x => x.ServiceBranchId == branchId)
            .AsQueryable();

        if (request.DayOfWeek.HasValue)
        {
            query = query.Where(x => x.DayOfWeek == request.DayOfWeek.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        query = request.SortBy?.ToLower() switch
        {
            "capacity" => request.Desc
                ? query.OrderByDescending(x => x.Capacity).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Capacity).ThenBy(x => x.Id),

            "dayofweek" => request.Desc
                ? query.OrderByDescending(x => x.DayOfWeek).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.DayOfWeek).ThenBy(x => x.Id),

            "createdat" => request.Desc
                ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.Id)
        };

        var totalRecords = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new BranchCapacityRuleResponse
            {
                Id = x.Id,
                ServiceBranchId = x.ServiceBranchId,
                BranchName = x.ServiceBranch.Name,
                DayOfWeek = x.DayOfWeek,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Capacity = x.Capacity,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new ApiResponse<PagedResponse<BranchCapacityRuleResponse>>
        {
            Success = true,
            Message = "Branch capacity rules retrieved successfully.",
            Data = new PagedResponse<BranchCapacityRuleResponse>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecords
            }
        };
    }

    public async Task<ApiResponse<BranchCapacityRuleResponse>> GetCapacityRuleByIdAsync(
    int branchId,
    int capacityRuleId,
    CancellationToken cancellationToken = default)
    {
        var rule = await _context.BranchCapacityRules
            .AsNoTracking()
            .Include(x => x.ServiceBranch)
            .FirstOrDefaultAsync(x =>
                x.Id == capacityRuleId &&
                x.ServiceBranchId == branchId,
                cancellationToken);

        if (rule is null)
        {
            return new ApiResponse<BranchCapacityRuleResponse>
            {
                Success = false,
                Message = "Branch capacity rule was not found."
            };
        }

        return new ApiResponse<BranchCapacityRuleResponse>
        {
            Success = true,
            Message = "Branch capacity rule retrieved successfully.",
            Data = ToBranchCapacityRuleResponse(rule)
        };
    }

    public async Task<ApiResponse<BranchCapacityRuleResponse>> CreateCapacityRuleAsync(
    int branchId,
    CreateBranchCapacityRuleRequest request,
    CancellationToken cancellationToken = default)
    {
        var branchExists = await _context.ServiceBranches
            .AnyAsync(x => x.Id == branchId, cancellationToken);

        if (!branchExists)
        {
            return new ApiResponse<BranchCapacityRuleResponse>
            {
                Success = false,
                Message = "Service branch was not found."
            };
        }

        var validationResult = ValidateCapacityRuleInput(
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.Capacity);

        if (!validationResult.Success)
        {
            return new ApiResponse<BranchCapacityRuleResponse>
            {
                Success = false,
                Message = validationResult.Message
            };
        }

        var duplicateExists = await _context.BranchCapacityRules
            .AnyAsync(x =>
                x.ServiceBranchId == branchId &&
                x.DayOfWeek == request.DayOfWeek &&
                x.StartTime == request.StartTime &&
                x.EndTime == request.EndTime,
                cancellationToken);

        if (duplicateExists)
        {
            return new ApiResponse<BranchCapacityRuleResponse>
            {
                Success = false,
                Message = "A branch capacity rule with the same schedule already exists."
            };
        }

        var rule = new BranchCapacityRule
        {
            ServiceBranchId = branchId,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Capacity = request.Capacity,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _context.BranchCapacityRules.AddAsync(rule, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.BranchCapacityRules
            .AsNoTracking()
            .Include(x => x.ServiceBranch)
            .FirstAsync(x => x.Id == rule.Id, cancellationToken);

        return new ApiResponse<BranchCapacityRuleResponse>
        {
            Success = true,
            Message = "Branch capacity rule created successfully.",
            Data = ToBranchCapacityRuleResponse(created)
        };
    }

    public async Task<ApiResponse<BranchCapacityRuleResponse>> UpdateCapacityRuleAsync(
    int branchId,
    int capacityRuleId,
    UpdateBranchCapacityRuleRequest request,
    CancellationToken cancellationToken = default)
    {
        var rule = await _context.BranchCapacityRules
            .Include(x => x.ServiceBranch)
            .FirstOrDefaultAsync(x =>
                x.Id == capacityRuleId &&
                x.ServiceBranchId == branchId,
                cancellationToken);

        if (rule is null)
        {
            return new ApiResponse<BranchCapacityRuleResponse>
            {
                Success = false,
                Message = "Branch capacity rule was not found."
            };
        }

        var validationResult = ValidateCapacityRuleInput(
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.Capacity);

        if (!validationResult.Success)
        {
            return new ApiResponse<BranchCapacityRuleResponse>
            {
                Success = false,
                Message = validationResult.Message
            };
        }

        var duplicateExists = await _context.BranchCapacityRules
            .AnyAsync(x =>
                x.Id != capacityRuleId &&
                x.ServiceBranchId == branchId &&
                x.DayOfWeek == request.DayOfWeek &&
                x.StartTime == request.StartTime &&
                x.EndTime == request.EndTime,
                cancellationToken);

        if (duplicateExists)
        {
            return new ApiResponse<BranchCapacityRuleResponse>
            {
                Success = false,
                Message = "A branch capacity rule with the same schedule already exists."
            };
        }

        rule.DayOfWeek = request.DayOfWeek;
        rule.StartTime = request.StartTime;
        rule.EndTime = request.EndTime;
        rule.Capacity = request.Capacity;
        rule.IsActive = request.IsActive;
        rule.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new ApiResponse<BranchCapacityRuleResponse>
        {
            Success = true,
            Message = "Branch capacity rule updated successfully.",
            Data = ToBranchCapacityRuleResponse(rule)
        };
    }

    public async Task<ApiResponse<string>> DeleteCapacityRuleAsync(
    int branchId,
    int capacityRuleId,
    CancellationToken cancellationToken = default)
    {
        var rule = await _context.BranchCapacityRules
            .FirstOrDefaultAsync(x =>
                x.Id == capacityRuleId &&
                x.ServiceBranchId == branchId,
                cancellationToken);

        if (rule is null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Branch capacity rule was not found."
            };
        }

        _context.BranchCapacityRules.Remove(rule);
        await _context.SaveChangesAsync(cancellationToken);

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Branch capacity rule deleted successfully.",
            Data = "Deleted"
        };
    }

    public async Task<int> GetCapacityForSlotAsync(
    int branchId,
    DateTime slotStart,
    DateTime slotEnd,
    CancellationToken cancellationToken = default)
    {
        var rules = await _context.BranchCapacityRules
            .AsNoTracking()
            .Where(x =>
                x.ServiceBranchId == branchId &&
                x.IsActive)
            .ToListAsync(cancellationToken);

        if (rules.Count == 0)
        {
            return 1;
        }

        var slotStartTime = slotStart.TimeOfDay;
        var slotEndTime = slotEnd.TimeOfDay;

        var exactTimeRule = rules
            .Where(x =>
                x.DayOfWeek == slotStart.DayOfWeek &&
                x.StartTime.HasValue &&
                x.EndTime.HasValue &&
                slotStartTime >= x.StartTime.Value &&
                slotEndTime <= x.EndTime.Value)
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();

        if (exactTimeRule is not null)
        {
            return exactTimeRule.Capacity;
        }

        var dayRule = rules
            .Where(x =>
                x.DayOfWeek == slotStart.DayOfWeek &&
                !x.StartTime.HasValue &&
                !x.EndTime.HasValue)
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();

        if (dayRule is not null)
        {
            return dayRule.Capacity;
        }

        var defaultRule = rules
            .Where(x =>
                !x.DayOfWeek.HasValue &&
                !x.StartTime.HasValue &&
                !x.EndTime.HasValue)
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();

        return defaultRule?.Capacity ?? 1;
    }

    private static ApiResponse<string> ValidateCapacityRuleInput(
    DayOfWeek? dayOfWeek,
    TimeSpan? startTime,
    TimeSpan? endTime,
    int capacity)
    {
        if (capacity <= 0)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Capacity must be greater than zero."
            };
        }

        if (startTime.HasValue != endTime.HasValue)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "StartTime and EndTime must be provided together."
            };
        }

        if (startTime.HasValue && !dayOfWeek.HasValue)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "DayOfWeek is required when StartTime and EndTime are provided."
            };
        }

        if (startTime.HasValue && startTime.Value >= endTime!.Value)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "StartTime must be before EndTime."
            };
        }

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Valid"
        };
    }

    private static BranchCapacityRuleResponse ToBranchCapacityRuleResponse(
    BranchCapacityRule rule)
    {
        return new BranchCapacityRuleResponse
        {
            Id = rule.Id,
            ServiceBranchId = rule.ServiceBranchId,
            BranchName = rule.ServiceBranch.Name,
            DayOfWeek = rule.DayOfWeek,
            StartTime = rule.StartTime,
            EndTime = rule.EndTime,
            Capacity = rule.Capacity,
            IsActive = rule.IsActive,
            CreatedAt = rule.CreatedAt,
            UpdatedAt = rule.UpdatedAt
        };
    }

    private static ApiResponse<string> ValidateClosureInput(
    DateTime startDate,
    DateTime endDate,
    bool isFullDay,
    TimeSpan? startTime,
    TimeSpan? endTime)
    {
        if (startDate.Date > endDate.Date)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "StartDate cannot be after EndDate."
            };
        }

        if (!isFullDay)
        {
            if (!startTime.HasValue || !endTime.HasValue)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "StartTime and EndTime are required for partial-day closures."
                };
            }

            if (startTime.Value >= endTime.Value)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "StartTime must be before EndTime."
                };
            }
        }

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Valid"
        };
    }

    private static BranchClosureResponse ToBranchClosureResponse(BranchClosure closure)
    {
        return new BranchClosureResponse
        {
            Id = closure.Id,
            ServiceBranchId = closure.ServiceBranchId,
            BranchName = closure.ServiceBranch.Name,
            StartDate = closure.StartDate,
            EndDate = closure.EndDate,
            IsFullDay = closure.IsFullDay,
            StartTime = closure.StartTime,
            EndTime = closure.EndTime,
            Type = closure.Type,
            Reason = closure.Reason,
            IsActive = closure.IsActive,
            CreatedAt = closure.CreatedAt,
            UpdatedAt = closure.UpdatedAt
        };
    }

    private static ServiceBranchResponse ToResponse(ServiceBranch branch)
    {
        return new ServiceBranchResponse
        {
            Id = branch.Id,
            Name = branch.Name,
            CountryCode = branch.CountryCode,
            City = branch.City,
            Latitude = branch.Latitude,
            Longitude = branch.Longitude,
            IsActive = branch.IsActive,
            ActiveServicesCount = branch.BranchServices.Count(x => x.IsActive),
            CreatedAt = branch.CreatedAt,
            UpdatedAt = branch.UpdatedAt
        };
    }

    private static BranchServiceResponse ToBranchServiceResponse(BranchService branchService)
    {
        return new BranchServiceResponse
        {
            Id = branchService.Id,
            ServiceBranchId = branchService.ServiceBranchId,
            BranchName = branchService.ServiceBranch.Name,
            ServiceId = branchService.ServiceId,
            ServiceName = branchService.Service.Name,
            IsActive = branchService.IsActive,
            CreatedAt = branchService.CreatedAt,
            UpdatedAt = branchService.UpdatedAt
        };
    }

    private static double CalculateDistanceKm(
        double latitude1,
        double longitude1,
        double latitude2,
        double longitude2)
    {
        const double earthRadiusKm = 6371;

        var dLat = DegreesToRadians(latitude2 - latitude1);
        var dLon = DegreesToRadians(longitude2 - longitude1);

        var lat1 = DegreesToRadians(latitude1);
        var lat2 = DegreesToRadians(latitude2);

        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2) *
            Math.Cos(lat1) * Math.Cos(lat2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    private static string NormalizeCountryCode(string countryCode)
    {
        var value = countryCode.Trim().ToUpperInvariant();

        if (value.Length == 2)
        {
            return value;
        }

        if (value.Length == 3)
        {
            var region = CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .Select(culture => new RegionInfo(culture.Name))
                .FirstOrDefault(region =>
                    string.Equals(
                        region.ThreeLetterISORegionName,
                        value,
                        StringComparison.OrdinalIgnoreCase));

            if (region is not null)
            {
                return region.TwoLetterISORegionName;
            }
        }

        return value;
    }

    private static string NormalizeCity(string city)
    {
        return city.Trim().ToLowerInvariant();
    }

    private static string NormalizeCityForStorage(string city)
    {
        return city.Trim();
    }
}