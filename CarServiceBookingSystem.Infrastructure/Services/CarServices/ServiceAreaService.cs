using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.ServiceAreas;
using CarServiceBookingSystem.Application.Interfaces.IServices;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarServiceBookingSystem.Infrastructure.Services.CarServices;

public class ServiceAreaService : IServiceAreaService
{
    private readonly ApplicationDbContext _context;

    public ServiceAreaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PagedResponse<ServiceAreaRuleResponse>>> GetRulesAsync(
        ServiceAreaRuleFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _context.ServiceAreaRules
            .AsNoTracking()
            .Include(x => x.Service)
            .AsQueryable();

        if (request.ServiceId.HasValue)
        {
            query = query.Where(x => x.ServiceId == request.ServiceId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.CountryCode))
        {
            var countryCode = NormalizeCountryCode(request.CountryCode);
            query = query.Where(x => x.CountryCode == countryCode);
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var city = NormalizeCity(request.City);
            query = query.Where(x => x.City != null && x.City.ToLower() == city);
        }

        if (request.IsAllowed.HasValue)
        {
            query = query.Where(x => x.IsAllowed == request.IsAllowed.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();

            query = query.Where(x =>
                x.CountryCode.ToLower().Contains(search) ||
                (x.City != null && x.City.ToLower().Contains(search)) ||
                (x.Service != null && x.Service.Name.ToLower().Contains(search)));
        }

        query = request.SortBy?.ToLower() switch
        {
            "country" => request.Desc
                ? query.OrderByDescending(x => x.CountryCode).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CountryCode).ThenBy(x => x.Id),

            "city" => request.Desc
                ? query.OrderByDescending(x => x.City).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.City).ThenBy(x => x.Id),

            "service" => request.Desc
                ? query.OrderByDescending(x => x.Service!.Name).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Service!.Name).ThenBy(x => x.Id),

            "priority" => request.Desc
                ? query.OrderByDescending(x => x.Priority).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Priority).ThenBy(x => x.Id),

            "isallowed" => request.Desc
                ? query.OrderByDescending(x => x.IsAllowed).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.IsAllowed).ThenBy(x => x.Id),

            "createdat" => request.Desc
                ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.Priority).ThenByDescending(x => x.Id)
        };

        var totalRecords = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ServiceAreaRuleResponse
            {
                Id = x.Id,
                ServiceId = x.ServiceId,
                ServiceName = x.Service != null ? x.Service.Name : null,
                CountryCode = x.CountryCode,
                City = x.City,
                IsAllowed = x.IsAllowed,
                Priority = x.Priority,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResponse<ServiceAreaRuleResponse>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalRecords
        };

        return new ApiResponse<PagedResponse<ServiceAreaRuleResponse>>
        {
            Success = true,
            Message = "Service area rules retrieved successfully.",
            Data = pagedResponse
        };
    }

    public async Task<ApiResponse<ServiceAreaRuleResponse>> GetRuleByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var rule = await _context.ServiceAreaRules
            .AsNoTracking()
            .Include(x => x.Service)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (rule is null)
        {
            return new ApiResponse<ServiceAreaRuleResponse>
            {
                Success = false,
                Message = "Service area rule was not found."
            };
        }

        return new ApiResponse<ServiceAreaRuleResponse>
        {
            Success = true,
            Message = "Service area rule retrieved successfully.",
            Data = ToResponse(rule)
        };
    }

    public async Task<ApiResponse<ServiceAreaRuleResponse>> CreateRuleAsync(
        CreateServiceAreaRuleRequest request,
        CancellationToken cancellationToken = default)
    {
        var countryCode = NormalizeCountryCode(request.CountryCode);
        var city = NormalizeCityOrNull(request.City);

        var validationResult = await ValidateRuleAsync(
            request.ServiceId,
            countryCode,
            city,
            excludedRuleId: null,
            cancellationToken);

        if (!validationResult.Success)
        {
            return new ApiResponse<ServiceAreaRuleResponse>
            {
                Success = false,
                Message = validationResult.Message
            };
        }

        var rule = new ServiceAreaRule
        {
            ServiceId = request.ServiceId,
            CountryCode = countryCode,
            City = city,
            IsAllowed = request.IsAllowed,
            Priority = request.Priority,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _context.ServiceAreaRules.AddAsync(rule, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var createdRule = await _context.ServiceAreaRules
            .AsNoTracking()
            .Include(x => x.Service)
            .FirstAsync(x => x.Id == rule.Id, cancellationToken);

        return new ApiResponse<ServiceAreaRuleResponse>
        {
            Success = true,
            Message = "Service area rule created successfully.",
            Data = ToResponse(createdRule)
        };
    }

    public async Task<ApiResponse<ServiceAreaRuleResponse>> UpdateRuleAsync(
        int id,
        UpdateServiceAreaRuleRequest request,
        CancellationToken cancellationToken = default)
    {
        var rule = await _context.ServiceAreaRules
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (rule is null)
        {
            return new ApiResponse<ServiceAreaRuleResponse>
            {
                Success = false,
                Message = "Service area rule was not found."
            };
        }

        var countryCode = NormalizeCountryCode(request.CountryCode);
        var city = NormalizeCityOrNull(request.City);

        var validationResult = await ValidateRuleAsync(
            request.ServiceId,
            countryCode,
            city,
            excludedRuleId: id,
            cancellationToken);

        if (!validationResult.Success)
        {
            return new ApiResponse<ServiceAreaRuleResponse>
            {
                Success = false,
                Message = validationResult.Message
            };
        }

        rule.ServiceId = request.ServiceId;
        rule.CountryCode = countryCode;
        rule.City = city;
        rule.IsAllowed = request.IsAllowed;
        rule.Priority = request.Priority;
        rule.IsActive = request.IsActive;
        rule.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var updatedRule = await _context.ServiceAreaRules
            .AsNoTracking()
            .Include(x => x.Service)
            .FirstAsync(x => x.Id == id, cancellationToken);

        return new ApiResponse<ServiceAreaRuleResponse>
        {
            Success = true,
            Message = "Service area rule updated successfully.",
            Data = ToResponse(updatedRule)
        };
    }

    public async Task<ApiResponse<string>> DeleteRuleAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var rule = await _context.ServiceAreaRules
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (rule is null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Service area rule was not found."
            };
        }

        _context.ServiceAreaRules.Remove(rule);
        await _context.SaveChangesAsync(cancellationToken);

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Service area rule deleted successfully.",
            Data = "Deleted"
        };
    }

    public async Task<ApiResponse<ServiceAreaCheckResponse>> CheckAvailabilityAsync(
        ServiceAreaCheckRequest request,
        CancellationToken cancellationToken = default)
    {
        var countryCode = NormalizeCountryCode(request.CountryCode);
        var city = NormalizeCityOrNull(request.City);

        var serviceExists = await _context.Services
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.ServiceId && x.IsActive, cancellationToken);

        if (!serviceExists)
        {
            return new ApiResponse<ServiceAreaCheckResponse>
            {
                Success = false,
                Message = "Service was not found."
            };
        }

        var candidateRules = await _context.ServiceAreaRules
            .AsNoTracking()
            .Where(x =>
                x.IsActive &&
                x.CountryCode == countryCode &&
                (x.ServiceId == null || x.ServiceId == request.ServiceId) &&
                (
                    x.City == null ||
                    (city != null && x.City.ToLower() == city)
                ))
            .ToListAsync(cancellationToken);

        var matchedRule = FindBestRule(candidateRules, request.ServiceId, city);

        if (matchedRule is null)
        {
            return new ApiResponse<ServiceAreaCheckResponse>
            {
                Success = true,
                Message = "Service area checked successfully.",
                Data = new ServiceAreaCheckResponse
                {
                    ServiceId = request.ServiceId,
                    CountryCode = countryCode,
                    City = city,
                    IsAvailable = false,
                    Reason = "No active service area rule matched this location.",
                    MatchedRuleId = null,
                    MatchedRuleScope = null
                }
            };
        }

        var scope = GetRuleScope(matchedRule);

        return new ApiResponse<ServiceAreaCheckResponse>
        {
            Success = true,
            Message = "Service area checked successfully.",
            Data = new ServiceAreaCheckResponse
            {
                ServiceId = request.ServiceId,
                CountryCode = countryCode,
                City = city,
                IsAvailable = matchedRule.IsAllowed,
                Reason = matchedRule.IsAllowed
                    ? $"Service is available for this location. Matched rule: {scope}."
                    : $"Service is not available for this location. Matched rule: {scope}.",
                MatchedRuleId = matchedRule.Id,
                MatchedRuleScope = scope
            }
        };
    }

    private async Task<ApiResponse<string>> ValidateRuleAsync(
        int? serviceId,
        string countryCode,
        string? city,
        int? excludedRuleId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "CountryCode is required."
            };
        }

        if (serviceId.HasValue)
        {
            var serviceExists = await _context.Services
                .AnyAsync(x => x.Id == serviceId.Value, cancellationToken);

            if (!serviceExists)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Service was not found."
                };
            }
        }

        var duplicateQuery = _context.ServiceAreaRules
            .AsNoTracking()
            .Where(x =>
                x.ServiceId == serviceId &&
                x.CountryCode == countryCode &&
                x.City == city);

        if (excludedRuleId.HasValue)
        {
            duplicateQuery = duplicateQuery.Where(x => x.Id != excludedRuleId.Value);
        }

        var duplicateExists = await duplicateQuery.AnyAsync(cancellationToken);

        if (duplicateExists)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "A service area rule with the same service, country, and city already exists."
            };
        }

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Valid"
        };
    }

    private static ServiceAreaRule? FindBestRule(
        List<ServiceAreaRule> rules,
        int serviceId,
        string? city)
    {
        var serviceCityRule = rules
            .Where(x =>
                x.ServiceId == serviceId &&
                city != null &&
                x.City != null &&
                NormalizeCity(x.City) == city)
            .OrderByDescending(x => x.Priority)
            .ThenByDescending(x => x.Id)
            .FirstOrDefault();

        if (serviceCityRule is not null)
        {
            return serviceCityRule;
        }

        var serviceCountryRule = rules
            .Where(x =>
                x.ServiceId == serviceId &&
                x.City == null)
            .OrderByDescending(x => x.Priority)
            .ThenByDescending(x => x.Id)
            .FirstOrDefault();

        if (serviceCountryRule is not null)
        {
            return serviceCountryRule;
        }

        var globalCityRule = rules
            .Where(x =>
                x.ServiceId == null &&
                city != null &&
                x.City != null &&
                NormalizeCity(x.City) == city)
            .OrderByDescending(x => x.Priority)
            .ThenByDescending(x => x.Id)
            .FirstOrDefault();

        if (globalCityRule is not null)
        {
            return globalCityRule;
        }

        var globalCountryRule = rules
            .Where(x =>
                x.ServiceId == null &&
                x.City == null)
            .OrderByDescending(x => x.Priority)
            .ThenByDescending(x => x.Id)
            .FirstOrDefault();

        return globalCountryRule;
    }

    private static ServiceAreaRuleResponse ToResponse(ServiceAreaRule rule)
    {
        return new ServiceAreaRuleResponse
        {
            Id = rule.Id,
            ServiceId = rule.ServiceId,
            ServiceName = rule.Service?.Name,
            CountryCode = rule.CountryCode,
            City = rule.City,
            IsAllowed = rule.IsAllowed,
            Priority = rule.Priority,
            IsActive = rule.IsActive,
            CreatedAt = rule.CreatedAt,
            UpdatedAt = rule.UpdatedAt
        };
    }

    private static string GetRuleScope(ServiceAreaRule rule)
    {
        if (rule.ServiceId.HasValue && !string.IsNullOrWhiteSpace(rule.City))
        {
            return "Service-specific city rule";
        }

        if (rule.ServiceId.HasValue)
        {
            return "Service-specific country rule";
        }

        if (!string.IsNullOrWhiteSpace(rule.City))
        {
            return "Global city rule";
        }

        return "Global country rule";
    }

    //private static string NormalizeCountryCode(string countryCode)
    //{
    //    return countryCode.Trim().ToUpper();
    //}

    private static string NormalizeCity(string city)
    {
        return city.Trim().ToLower();
    }

    private static string? NormalizeCityOrNull(string? city)
    {
        return string.IsNullOrWhiteSpace(city)
            ? null
            : NormalizeCity(city);
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
}