using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Cars;
using CarServiceBookingSystem.Application.DTOs.Services;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class ServiceService : IServiceService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private const string ServicesCachePrefix = "services-list";

    public ServiceService(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    private const string ServicesCacheVersionKey = "services-cache-version";

    private int GetCacheVersion()
    {
        return _cache.GetOrCreate(ServicesCacheVersionKey, entry => 1);
    }

    private void InvalidateServicesCache()
    {
        var currentVersion = GetCacheVersion();
        _cache.Set(ServicesCacheVersionKey, currentVersion + 1);
    }

    public async Task<ApiResponse<PagedResponse<ServiceResponse>>> GetAllAsync(PagedRequest request)
    {
        var version = GetCacheVersion();

        var cacheKey =
            $"{ServicesCachePrefix}-v{version}-{request.PageNumber}-{request.PageSize}-{request.Search}-{request.SortBy}-{request.Desc}";

        if (_cache.TryGetValue(cacheKey, out PagedResponse<ServiceResponse>? cachedResult))
        {
            return ApiResponse<PagedResponse<ServiceResponse>>.Ok(cachedResult!);
        }

        var query = _context.Services
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x => x.Name.Contains(search));
        }

        query = request.SortBy?.ToLower() switch
        {
            "name" => request.Desc
                ? query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Name).ThenBy(x => x.Id),

            "price" => request.Desc
                ? query.OrderByDescending(x => x.Price).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Price).ThenBy(x => x.Id),

            "duration" => request.Desc
                ? query.OrderByDescending(x => x.DurationInMinutes).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.DurationInMinutes).ThenBy(x => x.Id),

            _ => query.OrderBy(x => x.Name).ThenBy(x => x.Id)
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ServiceResponse
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                DurationInMinutes = x.DurationInMinutes
            })
            .ToListAsync();

        var response = new PagedResponse<ServiceResponse>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        _cache.Set(cacheKey, response, TimeSpan.FromMinutes(10));

        return ApiResponse<PagedResponse<ServiceResponse>>.Ok(response);
    }

    public async Task<ApiResponse<ServiceResponse>> CreateAsync(CreateServiceRequest request)
    {
        var service = new Service
        {
            Name = request.Name,
            Price = request.Price,
            DurationInMinutes = request.DurationInMinutes
        };

        await _context.Services.AddAsync(service);
        await _context.SaveChangesAsync();

        InvalidateServicesCache();

        return ApiResponse<ServiceResponse>.Ok(new ServiceResponse
        {
            Id = service.Id,
            Name = service.Name,
            Price = service.Price,
            DurationInMinutes = service.DurationInMinutes
        }, "Service created successfully");
    }

    public async Task<ApiResponse<ServiceResponse>> UpdateAsync(int id, UpdateServiceRequest request)
    {
        var service = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);

        if (service == null)
        {
            return ApiResponse<ServiceResponse>.Fail("Service not found");
        }

        service.Name = request.Name;
        service.Price = request.Price;
        service.DurationInMinutes = request.DurationInMinutes;

        await _context.SaveChangesAsync();
        InvalidateServicesCache();

        return ApiResponse<ServiceResponse>.Ok(new ServiceResponse
        {
            Id = service.Id,
            Name = service.Name,
            Price = service.Price,
            DurationInMinutes = service.DurationInMinutes
        }, "Service updated successfully");
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var service = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);

        if (service == null)
        {
            return ApiResponse<string>.Fail("Service not found");
        }

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
        InvalidateServicesCache();

        return ApiResponse<string>.Ok("Deleted", "Service deleted successfully");
    }
}