using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.PromoCodes;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class PromoCodeService : IPromoCodeService
{
    private readonly ApplicationDbContext _context;

    public PromoCodeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PagedResponse<PromoCodeResponse>>> GetAllAsync(
        PromoCodeQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _context.PromoCodes
            .AsNoTracking()
            .Include(x => x.Service)
            .Include(x => x.ServiceBranch)
            .Include(x => x.Redemptions)
            .AsQueryable();

        if (request.ServiceId.HasValue)
            query = query.Where(x => x.ServiceId == request.ServiceId.Value);

        if (request.ServiceBranchId.HasValue)
            query = query.Where(x => x.ServiceBranchId == request.ServiceBranchId.Value);

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.Code.Contains(search) ||
                (x.Description != null && x.Description.Contains(search)));
        }

        query = request.SortBy?.ToLower() switch
        {
            "code" => request.Desc
                ? query.OrderByDescending(x => x.Code).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Code).ThenBy(x => x.Id),

            "createdat" => request.Desc
                ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

            "expiresat" => request.Desc
                ? query.OrderByDescending(x => x.ExpiresAt).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.ExpiresAt).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.Id)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => MapToResponse(x))
            .ToListAsync(cancellationToken);

        var response = new PagedResponse<PromoCodeResponse>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return ApiResponse<PagedResponse<PromoCodeResponse>>.Ok(response);
    }

    public async Task<ApiResponse<PromoCodeResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var promoCode = await _context.PromoCodes
            .AsNoTracking()
            .Include(x => x.Service)
            .Include(x => x.ServiceBranch)
            .Include(x => x.Redemptions)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (promoCode is null)
        {
            return ApiResponse<PromoCodeResponse>.Fail("Promo code not found.");
        }

        return ApiResponse<PromoCodeResponse>.Ok(MapToResponse(promoCode));
    }

    public async Task<ApiResponse<PromoCodeResponse>> CreateAsync(
        CreatePromoCodeRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<DiscountType>(request.DiscountType, true, out var discountType))
        {
            return ApiResponse<PromoCodeResponse>.Fail("Invalid discount type.");
        }

        var normalizedCode = NormalizeCode(request.Code);

        var duplicateExists = await _context.PromoCodes
            .AsNoTracking()
            .AnyAsync(x => x.Code == normalizedCode, cancellationToken);

        if (duplicateExists)
        {
            return ApiResponse<PromoCodeResponse>.Fail("Promo code already exists.");
        }

        var validationResult = await ValidateReferencesAsync(
            request.ServiceId,
            request.ServiceBranchId,
            cancellationToken);

        if (!validationResult.Success)
        {
            return ApiResponse<PromoCodeResponse>.Fail(validationResult.Message!);
        }

        var promoCode = new PromoCode
        {
            Code = normalizedCode,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            DiscountType = discountType,
            DiscountValue = request.DiscountValue,
            MaxDiscountAmount = request.MaxDiscountAmount,
            MinimumSubtotalAmount = request.MinimumSubtotalAmount,
            ServiceId = request.ServiceId,
            ServiceBranchId = request.ServiceBranchId,
            StartsAt = request.StartsAt,
            ExpiresAt = request.ExpiresAt,
            MaxTotalRedemptions = request.MaxTotalRedemptions,
            MaxRedemptionsPerUser = request.MaxRedemptionsPerUser,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.PromoCodes.Add(promoCode);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(promoCode.Id, cancellationToken);
    }

    public async Task<ApiResponse<PromoCodeResponse>> UpdateAsync(
        int id,
        UpdatePromoCodeRequest request,
        CancellationToken cancellationToken = default)
    {
        var promoCode = await _context.PromoCodes
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (promoCode is null)
        {
            return ApiResponse<PromoCodeResponse>.Fail("Promo code not found.");
        }

        if (!Enum.TryParse<DiscountType>(request.DiscountType, true, out var discountType))
        {
            return ApiResponse<PromoCodeResponse>.Fail("Invalid discount type.");
        }

        var normalizedCode = NormalizeCode(request.Code);

        var duplicateExists = await _context.PromoCodes
            .AsNoTracking()
            .AnyAsync(x => x.Id != id && x.Code == normalizedCode, cancellationToken);

        if (duplicateExists)
        {
            return ApiResponse<PromoCodeResponse>.Fail("Promo code already exists.");
        }

        var validationResult = await ValidateReferencesAsync(
            request.ServiceId,
            request.ServiceBranchId,
            cancellationToken);

        if (!validationResult.Success)
        {
            return ApiResponse<PromoCodeResponse>.Fail(validationResult.Message!);
        }

        promoCode.Code = normalizedCode;
        promoCode.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        promoCode.DiscountType = discountType;
        promoCode.DiscountValue = request.DiscountValue;
        promoCode.MaxDiscountAmount = request.MaxDiscountAmount;
        promoCode.MinimumSubtotalAmount = request.MinimumSubtotalAmount;
        promoCode.ServiceId = request.ServiceId;
        promoCode.ServiceBranchId = request.ServiceBranchId;
        promoCode.StartsAt = request.StartsAt;
        promoCode.ExpiresAt = request.ExpiresAt;
        promoCode.MaxTotalRedemptions = request.MaxTotalRedemptions;
        promoCode.MaxRedemptionsPerUser = request.MaxRedemptionsPerUser;
        promoCode.IsActive = request.IsActive;
        promoCode.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(promoCode.Id, cancellationToken);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var promoCode = await _context.PromoCodes
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (promoCode is null)
        {
            return ApiResponse<bool>.Fail("Promo code not found.");
        }

        var hasRedemptions = await _context.PromoCodeRedemptions
            .AsNoTracking()
            .AnyAsync(x => x.PromoCodeId == id, cancellationToken);

        if (hasRedemptions)
        {
            promoCode.IsActive = false;
            promoCode.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Promo code has redemptions, so it was deactivated instead of deleted.");
        }

        _context.PromoCodes.Remove(promoCode);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }

    public async Task<PromoCodeValidationResponse> ValidateAsync(
        PromoCodeValidationRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = NormalizeCode(request.PromoCode);

        if (string.IsNullOrWhiteSpace(normalizedCode))
        {
            return Invalid("Promo code is required.");
        }

        var now = DateTime.UtcNow;

        var promoCode = await _context.PromoCodes
            .AsNoTracking()
            .Include(x => x.Redemptions)
            .FirstOrDefaultAsync(x => x.Code == normalizedCode, cancellationToken);

        if (promoCode is null)
        {
            return Invalid("Promo code not found.");
        }

        if (!promoCode.IsActive)
        {
            return Invalid("Promo code is inactive.");
        }

        if (promoCode.StartsAt.HasValue && promoCode.StartsAt.Value > now)
        {
            return Invalid("Promo code is not active yet.");
        }

        if (promoCode.ExpiresAt.HasValue && promoCode.ExpiresAt.Value < now)
        {
            return Invalid("Promo code has expired.");
        }

        if (promoCode.ServiceId.HasValue && promoCode.ServiceId.Value != request.ServiceId)
        {
            return Invalid("Promo code is not valid for this service.");
        }

        if (promoCode.ServiceBranchId.HasValue &&
            promoCode.ServiceBranchId.Value != request.ServiceBranchId)
        {
            return Invalid("Promo code is not valid for this branch.");
        }

        if (promoCode.MinimumSubtotalAmount.HasValue &&
            request.SubtotalPrice < promoCode.MinimumSubtotalAmount.Value)
        {
            return Invalid($"Minimum subtotal for this promo code is {promoCode.MinimumSubtotalAmount.Value}.");
        }

        if (promoCode.MaxTotalRedemptions.HasValue &&
            promoCode.Redemptions.Count >= promoCode.MaxTotalRedemptions.Value)
        {
            return Invalid("Promo code redemption limit has been reached.");
        }

        if (promoCode.MaxRedemptionsPerUser.HasValue)
        {
            var userRedemptions = promoCode.Redemptions
                .Count(x => x.UserId == request.UserId);

            if (userRedemptions >= promoCode.MaxRedemptionsPerUser.Value)
            {
                return Invalid("You have already used this promo code the maximum number of times.");
            }
        }

        var discountAmount = CalculateDiscountAmount(
            promoCode,
            request.SubtotalPrice);

        if (discountAmount <= 0)
        {
            return Invalid("Promo code does not produce a valid discount.");
        }

        return new PromoCodeValidationResponse
        {
            IsValid = true,
            PromoCodeId = promoCode.Id,
            PromoCode = promoCode.Code,
            DiscountAmount = discountAmount,
            Message = "Promo code applied."
        };
    }

    public async Task RedeemAsync(
        int promoCodeId,
        int bookingId,
        string userId,
        decimal discountAmount,
        CancellationToken cancellationToken = default)
    {
        var alreadyRedeemed = await _context.PromoCodeRedemptions
            .AsNoTracking()
            .AnyAsync(x => x.BookingId == bookingId, cancellationToken);

        if (alreadyRedeemed)
        {
            return;
        }

        var redemption = new PromoCodeRedemption
        {
            PromoCodeId = promoCodeId,
            BookingId = bookingId,
            UserId = userId,
            DiscountAmount = discountAmount,
            CreatedAt = DateTime.UtcNow
        };

        _context.PromoCodeRedemptions.Add(redemption);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<ApiResponse<bool>> ValidateReferencesAsync(
        int? serviceId,
        int? serviceBranchId,
        CancellationToken cancellationToken)
    {
        if (serviceId.HasValue)
        {
            var serviceExists = await _context.Services
                .AsNoTracking()
                .AnyAsync(x => x.Id == serviceId.Value, cancellationToken);

            if (!serviceExists)
            {
                return ApiResponse<bool>.Fail("Service not found.");
            }
        }

        if (serviceBranchId.HasValue)
        {
            var branchExists = await _context.ServiceBranches
                .AsNoTracking()
                .AnyAsync(x => x.Id == serviceBranchId.Value, cancellationToken);

            if (!branchExists)
            {
                return ApiResponse<bool>.Fail("Service branch not found.");
            }
        }

        return ApiResponse<bool>.Ok(true);
    }

    private static decimal CalculateDiscountAmount(
        PromoCode promoCode,
        decimal subtotalPrice)
    {
        decimal discountAmount;

        if (promoCode.DiscountType == DiscountType.Percentage)
        {
            discountAmount = subtotalPrice * promoCode.DiscountValue / 100m;
        }
        else
        {
            discountAmount = promoCode.DiscountValue;
        }

        if (promoCode.MaxDiscountAmount.HasValue &&
            discountAmount > promoCode.MaxDiscountAmount.Value)
        {
            discountAmount = promoCode.MaxDiscountAmount.Value;
        }

        if (discountAmount > subtotalPrice)
        {
            discountAmount = subtotalPrice;
        }

        return Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero);
    }

    private static string NormalizeCode(string code)
    {
        return string.IsNullOrWhiteSpace(code)
            ? string.Empty
            : code.Trim().ToUpperInvariant();
    }

    private static PromoCodeValidationResponse Invalid(string message)
    {
        return new PromoCodeValidationResponse
        {
            IsValid = false,
            DiscountAmount = 0,
            Message = message
        };
    }

    private static PromoCodeResponse MapToResponse(PromoCode promoCode)
    {
        return new PromoCodeResponse
        {
            Id = promoCode.Id,
            Code = promoCode.Code,
            Description = promoCode.Description,
            DiscountType = promoCode.DiscountType.ToString(),
            DiscountValue = promoCode.DiscountValue,
            MaxDiscountAmount = promoCode.MaxDiscountAmount,
            MinimumSubtotalAmount = promoCode.MinimumSubtotalAmount,
            ServiceId = promoCode.ServiceId,
            ServiceName = promoCode.Service?.Name,
            ServiceBranchId = promoCode.ServiceBranchId,
            ServiceBranchName = promoCode.ServiceBranch?.Name,
            StartsAt = promoCode.StartsAt,
            ExpiresAt = promoCode.ExpiresAt,
            MaxTotalRedemptions = promoCode.MaxTotalRedemptions,
            MaxRedemptionsPerUser = promoCode.MaxRedemptionsPerUser,
            IsActive = promoCode.IsActive,
            TotalRedemptions = promoCode.Redemptions.Count,
            CreatedAt = promoCode.CreatedAt,
            UpdatedAt = promoCode.UpdatedAt
        };
    }
}