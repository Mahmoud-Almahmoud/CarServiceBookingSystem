using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.ServicePricing;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class ServicePricingService : IServicePricingService
{
    private readonly ApplicationDbContext _context;

    public ServicePricingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PagedResponse<ServicePriceRuleResponse>>> GetRulesAsync(
        ServicePriceRuleFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _context.ServicePriceRules
            .AsNoTracking()
            .Include(x => x.Service)
            .Include(x => x.CarBrand)
            .Include(x => x.CarModel)
            .Include(x => x.CarYear)
            .Include(x => x.CarTrim)
            .AsQueryable();

        if (request.ServiceId.HasValue)
        {
            query = query.Where(x => x.ServiceId == request.ServiceId.Value);
        }

        if (request.CarBrandId.HasValue)
        {
            query = query.Where(x => x.CarBrandId == request.CarBrandId.Value);
        }

        if (request.CarModelId.HasValue)
        {
            query = query.Where(x => x.CarModelId == request.CarModelId.Value);
        }

        if (request.CarYearId.HasValue)
        {
            query = query.Where(x => x.CarYearId == request.CarYearId.Value);
        }

        if (request.CarTrimId.HasValue)
        {
            query = query.Where(x => x.CarTrimId == request.CarTrimId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();

            query = query.Where(x =>
                x.Service.Name.ToLower().Contains(search) ||
                (x.CarBrand != null && x.CarBrand.Name.ToLower().Contains(search)) ||
                (x.CarModel != null && x.CarModel.Name.ToLower().Contains(search)) ||
                (x.CarYear != null && x.CarYear.Year.ToString().Contains(search)) ||
                (x.CarTrim != null && x.CarTrim.Name.ToLower().Contains(search)));
        }

        query = request.SortBy?.ToLower() switch
        {
            "price" => request.Desc
                ? query.OrderByDescending(x => x.Price).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Price).ThenBy(x => x.Id),

            "duration" => request.Desc
                ? query.OrderByDescending(x => x.DurationMinutes).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.DurationMinutes).ThenBy(x => x.Id),

            "service" => request.Desc
                ? query.OrderByDescending(x => x.Service.Name).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Service.Name).ThenBy(x => x.Id),

            "brand" => request.Desc
                ? query.OrderByDescending(x => x.CarBrand!.Name).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CarBrand!.Name).ThenBy(x => x.Id),

            "model" => request.Desc
                ? query.OrderByDescending(x => x.CarModel!.Name).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CarModel!.Name).ThenBy(x => x.Id),

            "year" => request.Desc
                ? query.OrderByDescending(x => x.CarYear!.Year).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CarYear!.Year).ThenBy(x => x.Id),

            "trim" => request.Desc
                ? query.OrderByDescending(x => x.CarTrim!.Name).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CarTrim!.Name).ThenBy(x => x.Id),

            "createdat" => request.Desc
                ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.Id)
        };

        var totalRecords = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ServicePriceRuleResponse
            {
                Id = x.Id,
                ServiceId = x.ServiceId,
                ServiceName = x.Service.Name,

                CarBrandId = x.CarBrandId,
                CarBrandName = x.CarBrand != null ? x.CarBrand.Name : null,

                CarModelId = x.CarModelId,
                CarModelName = x.CarModel != null ? x.CarModel.Name : null,

                CarYearId = x.CarYearId,
                CarYearValue = x.CarYear != null ? x.CarYear.Year : null,

                CarTrimId = x.CarTrimId,
                CarTrimName = x.CarTrim != null ? x.CarTrim.Name : null,

                Price = x.Price,
                DurationMinutes = x.DurationMinutes,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResponse<ServicePriceRuleResponse>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalRecords
        };

        return new ApiResponse<PagedResponse<ServicePriceRuleResponse>>
        {
            Success = true,
            Message = "Service price rules retrieved successfully.",
            Data = pagedResponse
        };
    }

    public async Task<ApiResponse<ServicePriceRuleResponse>> GetRuleByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var rule = await _context.ServicePriceRules
            .AsNoTracking()
            .Include(x => x.Service)
            .Include(x => x.CarBrand)
            .Include(x => x.CarModel)
            .Include(x => x.CarYear)
            .Include(x => x.CarTrim)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (rule is null)
        {
            return new ApiResponse<ServicePriceRuleResponse>
            {
                Success = false,
                Message = "Service price rule was not found."
            };
        }

        return new ApiResponse<ServicePriceRuleResponse>
        {
            Success = true,
            Message = "Service price rule retrieved successfully.",
            Data = ToResponse(rule)
        };
    }

    public async Task<ApiResponse<ServicePriceRuleResponse>> CreateRuleAsync(
        CreateServicePriceRuleRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateRuleAsync(
            request.ServiceId,
            request.CarBrandId,
            request.CarModelId,
            request.CarYearId,
            request.CarTrimId,
            excludedRuleId: null,
            cancellationToken);

        if (!validationResult.Success)
        {
            return new ApiResponse<ServicePriceRuleResponse>
            {
                Success = false,
                Message = validationResult.Message
            };
        }

        var rule = new ServicePriceRule
        {
            ServiceId = request.ServiceId,
            CarBrandId = request.CarBrandId,
            CarModelId = request.CarModelId,
            CarYearId = request.CarYearId,
            CarTrimId = request.CarTrimId,
            Price = request.Price,
            DurationMinutes = request.DurationMinutes,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _context.ServicePriceRules.AddAsync(rule, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var createdRule = await _context.ServicePriceRules
            .AsNoTracking()
            .Include(x => x.Service)
            .Include(x => x.CarBrand)
            .Include(x => x.CarModel)
            .Include(x => x.CarYear)
            .Include(x => x.CarTrim)
            .FirstAsync(x => x.Id == rule.Id, cancellationToken);

        return new ApiResponse<ServicePriceRuleResponse>
        {
            Success = true,
            Message = "Service price rule created successfully.",
            Data = ToResponse(createdRule)
        };
    }

    public async Task<ApiResponse<ServicePriceRuleResponse>> UpdateRuleAsync(
        int id,
        UpdateServicePriceRuleRequest request,
        CancellationToken cancellationToken = default)
    {
        var rule = await _context.ServicePriceRules
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (rule is null)
        {
            return new ApiResponse<ServicePriceRuleResponse>
            {
                Success = false,
                Message = "Service price rule was not found."
            };
        }

        var validationResult = await ValidateRuleAsync(
            request.ServiceId,
            request.CarBrandId,
            request.CarModelId,
            request.CarYearId,
            request.CarTrimId,
            excludedRuleId: id,
            cancellationToken);

        if (!validationResult.Success)
        {
            return new ApiResponse<ServicePriceRuleResponse>
            {
                Success = false,
                Message = validationResult.Message
            };
        }

        rule.ServiceId = request.ServiceId;
        rule.CarBrandId = request.CarBrandId;
        rule.CarModelId = request.CarModelId;
        rule.CarYearId = request.CarYearId;
        rule.CarTrimId = request.CarTrimId;
        rule.Price = request.Price;
        rule.DurationMinutes = request.DurationMinutes;
        rule.IsActive = request.IsActive;
        rule.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var updatedRule = await _context.ServicePriceRules
            .AsNoTracking()
            .Include(x => x.Service)
            .Include(x => x.CarBrand)
            .Include(x => x.CarModel)
            .Include(x => x.CarYear)
            .Include(x => x.CarTrim)
            .FirstAsync(x => x.Id == id, cancellationToken);

        return new ApiResponse<ServicePriceRuleResponse>
        {
            Success = true,
            Message = "Service price rule updated successfully.",
            Data = ToResponse(updatedRule)
        };
    }

    public async Task<ApiResponse<string>> DeleteRuleAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var rule = await _context.ServicePriceRules
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (rule is null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Service price rule was not found."
            };
        }

        _context.ServicePriceRules.Remove(rule);
        await _context.SaveChangesAsync(cancellationToken);

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Service price rule deleted successfully.",
            Data = "Deleted"
        };
    }

    public async Task<ApiResponse<ServicePriceQuoteResponse>> GetPriceQuoteAsync(
        ServicePriceQuoteRequest request,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var car = await _context.Cars
            .AsNoTracking()
            .Include(x => x.CarTrim)
                .ThenInclude(x => x.Year)
                    .ThenInclude(x => x.Model)
                        .ThenInclude(x => x.Brand)
            .FirstOrDefaultAsync(x =>
                x.Id == request.CarId &&
                x.UserId == userId,
                cancellationToken);

        if (car is null)
        {
            return new ApiResponse<ServicePriceQuoteResponse>
            {
                Success = false,
                Message = "Car was not found."
            };
        }

        var service = await _context.Services
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == request.ServiceId &&
                x.IsActive,
                cancellationToken);

        if (service is null)
        {
            return new ApiResponse<ServicePriceQuoteResponse>
            {
                Success = false,
                Message = "Service was not found."
            };
        }

        var carTrimId = car.CarTrimId;
        var carYearId = car.CarTrim.YearId;
        var carModelId = car.CarTrim.Year.ModelId;
        var carBrandId = car.CarTrim.Year.Model.BrandId;

        var matchedRule = await _context.ServicePriceRules
            .AsNoTracking()
            .Where(x =>
                x.IsActive &&
                x.ServiceId == request.ServiceId &&

                (!x.CarBrandId.HasValue || x.CarBrandId == carBrandId) &&
                (!x.CarModelId.HasValue || x.CarModelId == carModelId) &&
                (!x.CarYearId.HasValue || x.CarYearId == carYearId) &&
                (!x.CarTrimId.HasValue || x.CarTrimId == carTrimId))
            .OrderByDescending(x => x.CarTrimId.HasValue)
            .ThenByDescending(x => x.CarYearId.HasValue)
            .ThenByDescending(x => x.CarModelId.HasValue)
            .ThenByDescending(x => x.CarBrandId.HasValue)
            .ThenByDescending(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var response = new ServicePriceQuoteResponse
        {
            ServiceId = service.Id,
            ServiceName = service.Name,
            CarId = car.Id,

            Price = matchedRule?.Price ?? service.Price,
            DurationMinutes = matchedRule?.DurationMinutes ?? service.DurationInMinutes,

            UsedCustomPriceRule = matchedRule is not null,
            ServicePriceRuleId = matchedRule?.Id,

            PricingSource = matchedRule is null
                ? "Service base price"
                : BuildPricingSource(matchedRule),

            CarBrandId = carBrandId,
            CarBrandName = car.CarTrim.Year.Model.Brand.Name,

            CarModelId = carModelId,
            CarModelName = car.CarTrim.Year.Model.Name,

            CarYearId = carYearId,
            CarYearValue = car.CarTrim.Year.Year,

            CarTrimId = carTrimId,
            CarTrimName = car.CarTrim.Name
        };

        return new ApiResponse<ServicePriceQuoteResponse>
        {
            Success = true,
            Message = "Service price quote calculated successfully.",
            Data = response
        };
    }

    private async Task<ApiResponse<string>> ValidateRuleAsync(
        int serviceId,
        int? carBrandId,
        int? carModelId,
        int? carYearId,
        int? carTrimId,
        int? excludedRuleId,
        CancellationToken cancellationToken)
    {
        var serviceExists = await _context.Services
            .AnyAsync(x => x.Id == serviceId, cancellationToken);

        if (!serviceExists)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Service was not found."
            };
        }

        if (!carBrandId.HasValue &&
            !carModelId.HasValue &&
            !carYearId.HasValue &&
            !carTrimId.HasValue)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "At least one pricing condition is required."
            };
        }

        if (carBrandId.HasValue)
        {
            var brandExists = await _context.CarBrands
                .AnyAsync(x => x.Id == carBrandId.Value, cancellationToken);

            if (!brandExists)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Car brand was not found."
                };
            }
        }

        if (carModelId.HasValue)
        {
            var modelExists = await _context.CarModels
                .AnyAsync(x => x.Id == carModelId.Value, cancellationToken);

            if (!modelExists)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Car model was not found."
                };
            }
        }

        if (carYearId.HasValue)
        {
            var yearExists = await _context.CarYears
                .AnyAsync(x => x.Id == carYearId.Value, cancellationToken);

            if (!yearExists)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Car year was not found."
                };
            }
        }

        if (carTrimId.HasValue)
        {
            var trimExists = await _context.CarTrims
                .AnyAsync(x => x.Id == carTrimId.Value, cancellationToken);

            if (!trimExists)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Car trim was not found."
                };
            }
        }

        if (carModelId.HasValue && carBrandId.HasValue)
        {
            var modelBelongsToBrand = await _context.CarModels
                .AnyAsync(x =>
                    x.Id == carModelId.Value &&
                    x.BrandId == carBrandId.Value,
                    cancellationToken);

            if (!modelBelongsToBrand)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Car model does not belong to the selected brand."
                };
            }
        }

        if (carYearId.HasValue && carModelId.HasValue)
        {
            var yearBelongsToModel = await _context.CarYears
                .AnyAsync(x =>
                    x.Id == carYearId.Value &&
                    x.ModelId == carModelId.Value,
                    cancellationToken);

            if (!yearBelongsToModel)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Car year does not belong to the selected model."
                };
            }
        }

        if (carTrimId.HasValue && carYearId.HasValue)
        {
            var trimBelongsToYear = await _context.CarTrims
                .AnyAsync(x =>
                    x.Id == carTrimId.Value &&
                    x.YearId == carYearId.Value,
                    cancellationToken);

            if (!trimBelongsToYear)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Car trim does not belong to the selected year."
                };
            }
        }

        var duplicateQuery = _context.ServicePriceRules
            .AsNoTracking()
            .Where(x =>
                x.ServiceId == serviceId &&
                x.CarBrandId == carBrandId &&
                x.CarModelId == carModelId &&
                x.CarYearId == carYearId &&
                x.CarTrimId == carTrimId);

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
                Message = "A service price rule with the same conditions already exists."
            };
        }

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Valid"
        };
    }

    private static ServicePriceRuleResponse ToResponse(ServicePriceRule rule)
    {
        return new ServicePriceRuleResponse
        {
            Id = rule.Id,

            ServiceId = rule.ServiceId,
            ServiceName = rule.Service.Name,

            CarBrandId = rule.CarBrandId,
            CarBrandName = rule.CarBrand?.Name,

            CarModelId = rule.CarModelId,
            CarModelName = rule.CarModel?.Name,

            CarYearId = rule.CarYearId,
            CarYearValue = rule.CarYear?.Year,

            CarTrimId = rule.CarTrimId,
            CarTrimName = rule.CarTrim?.Name,

            Price = rule.Price,
            DurationMinutes = rule.DurationMinutes,
            IsActive = rule.IsActive,
            CreatedAt = rule.CreatedAt,
            UpdatedAt = rule.UpdatedAt
        };
    }

    private static string BuildPricingSource(ServicePriceRule rule)
    {
        if (rule.CarTrimId.HasValue)
        {
            return "Trim-specific price rule";
        }

        if (rule.CarYearId.HasValue)
        {
            return "Year-specific price rule";
        }

        if (rule.CarModelId.HasValue)
        {
            return "Model-specific price rule";
        }

        if (rule.CarBrandId.HasValue)
        {
            return "Brand-specific price rule";
        }

        return "Custom service price rule";
    }
}