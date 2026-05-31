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