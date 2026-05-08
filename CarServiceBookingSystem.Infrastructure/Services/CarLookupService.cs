using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Cars;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class CarLookupService : ICarLookupService
{
    private readonly ApplicationDbContext _context;

    private readonly IMemoryCache _cache;

    public CarLookupService(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<ApiResponse<List<CarBrandDto>>> GetBrandsAsync()
    {
        var cacheKey = "car-brands";

        if (_cache.TryGetValue(cacheKey, out List<CarBrandDto>? cachedBrands))
            return ApiResponse<List<CarBrandDto>>.Ok(cachedBrands!);

        var brands = await _context.CarBrands
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new CarBrandDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

        _cache.Set(cacheKey, brands, TimeSpan.FromHours(6));

        return ApiResponse<List<CarBrandDto>>.Ok(brands);
    }

    public async Task<ApiResponse<List<CarModelDto>>> GetModelsByBrandAsync(int brandId)
    {
        var cacheKey = $"car-models-brand-{brandId}";

        if (_cache.TryGetValue(cacheKey, out List<CarModelDto>? cachedModels))
            return ApiResponse<List<CarModelDto>>.Ok(cachedModels!);

        var models = await _context.CarModels
            .AsNoTracking()
            .Where(x => x.BrandId == brandId)
            .OrderBy(x => x.Name)
            .Select(x => new CarModelDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

        _cache.Set(cacheKey, models, TimeSpan.FromHours(6));

        return ApiResponse<List<CarModelDto>>.Ok(models);
    }

    public async Task<ApiResponse<List<CarYearDto>>> GetYearsByModelAsync(int modelId)
    {
        var cacheKey = $"car-years-model-{modelId}";
        if (_cache.TryGetValue(cacheKey, out List<CarYearDto>? cachedYears))
            return ApiResponse<List<CarYearDto>>.Ok(cachedYears!);

        var years = await _context.CarYears
            .AsNoTracking()
            .Where(x => x.ModelId == modelId)
            .OrderByDescending(x => x.Year)
            .Select(x => new CarYearDto
            {
                Id = x.Id,
                Year = x.Year
            })
            .ToListAsync();

        _cache.Set(cacheKey, years, TimeSpan.FromHours(6));

        return ApiResponse<List<CarYearDto>>.Ok(years);
    }

    public async Task<ApiResponse<List<CarTrimDto>>> GetTrimsByYearAsync(int yearId)
    {
        var cacheKey = $"car-trims-year-{yearId}";
        if (_cache.TryGetValue(cacheKey, out List<CarTrimDto>? cachedTrims))
            return ApiResponse<List<CarTrimDto>>.Ok(cachedTrims!);

        var trims = await _context.CarTrims
            .AsNoTracking()
            .Where(x => x.YearId == yearId)
            .OrderBy(x => x.Name)
            .Select(x => new CarTrimDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

        _cache.Set(cacheKey, trims, TimeSpan.FromHours(6));

        return ApiResponse<List<CarTrimDto>>.Ok(trims);
    }
}