using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.CancellationPolicyRules;
using CarServiceBookingSystem.Application.Interfaces.IBookings;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services.Bookings;

public class CancellationPolicyRuleService : ICancellationPolicyRuleService
{
    private readonly ApplicationDbContext _context;

    public CancellationPolicyRuleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PagedResponse<CancellationPolicyRuleResponse>>> GetAllAsync(
        CancellationPolicyRuleQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _context.CancellationPolicyRules
            .AsNoTracking()
            .Include(x => x.Service)
            .Include(x => x.ServiceBranch)
            .AsQueryable();

        if (request.ServiceId.HasValue)
        {
            query = query.Where(x => x.ServiceId == request.ServiceId.Value);
        }

        if (request.ServiceBranchId.HasValue)
        {
            query = query.Where(x => x.ServiceBranchId == request.ServiceBranchId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        query = request.SortBy?.ToLower() switch
        {
            "hoursbeforestart" => request.Desc
                ? query.OrderByDescending(x => x.HoursBeforeStart).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.HoursBeforeStart).ThenBy(x => x.Id),

            "refundpercentage" => request.Desc
                ? query.OrderByDescending(x => x.RefundPercentage).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.RefundPercentage).ThenBy(x => x.Id),

            "createdat" => request.Desc
                ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.Id)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => MapToResponse(x))
            .ToListAsync(cancellationToken);

        var response = new PagedResponse<CancellationPolicyRuleResponse>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return ApiResponse<PagedResponse<CancellationPolicyRuleResponse>>.Ok(response);
    }

    public async Task<ApiResponse<CancellationPolicyRuleResponse>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var rule = await _context.CancellationPolicyRules
            .AsNoTracking()
            .Include(x => x.Service)
            .Include(x => x.ServiceBranch)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (rule is null)
        {
            return ApiResponse<CancellationPolicyRuleResponse>.Fail("Cancellation policy rule not found.");
        }

        return ApiResponse<CancellationPolicyRuleResponse>.Ok(MapToResponse(rule));
    }

    public async Task<ApiResponse<CancellationPolicyRuleResponse>> CreateAsync(
        CreateCancellationPolicyRuleRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResponse = await ValidateReferencesAsync(
            request.ServiceId,
            request.ServiceBranchId,
            cancellationToken);

        if (!validationResponse.Success)
        {
            return ApiResponse<CancellationPolicyRuleResponse>.Fail(validationResponse.Message!);
        }

        var duplicateExists = await _context.CancellationPolicyRules
            .AsNoTracking()
            .AnyAsync(x =>
                x.ServiceId == request.ServiceId &&
                x.ServiceBranchId == request.ServiceBranchId &&
                x.HoursBeforeStart == request.HoursBeforeStart,
                cancellationToken);

        if (duplicateExists)
        {
            return ApiResponse<CancellationPolicyRuleResponse>.Fail(
                "A cancellation policy rule already exists for this service/branch/hour threshold.");
        }

        var rule = new CancellationPolicyRule
        {
            ServiceId = request.ServiceId,
            ServiceBranchId = request.ServiceBranchId,
            HoursBeforeStart = request.HoursBeforeStart,
            RefundPercentage = request.RefundPercentage,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.CancellationPolicyRules.Add(rule);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(rule.Id, cancellationToken);
    }

    public async Task<ApiResponse<CancellationPolicyRuleResponse>> UpdateAsync(
        int id,
        UpdateCancellationPolicyRuleRequest request,
        CancellationToken cancellationToken = default)
    {
        var rule = await _context.CancellationPolicyRules
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (rule is null)
        {
            return ApiResponse<CancellationPolicyRuleResponse>.Fail("Cancellation policy rule not found.");
        }

        var validationResponse = await ValidateReferencesAsync(
            request.ServiceId,
            request.ServiceBranchId,
            cancellationToken);

        if (!validationResponse.Success)
        {
            return ApiResponse<CancellationPolicyRuleResponse>.Fail(validationResponse.Message!);
        }

        var duplicateExists = await _context.CancellationPolicyRules
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id != id &&
                x.ServiceId == request.ServiceId &&
                x.ServiceBranchId == request.ServiceBranchId &&
                x.HoursBeforeStart == request.HoursBeforeStart,
                cancellationToken);

        if (duplicateExists)
        {
            return ApiResponse<CancellationPolicyRuleResponse>.Fail(
                "A cancellation policy rule already exists for this service/branch/hour threshold.");
        }

        rule.ServiceId = request.ServiceId;
        rule.ServiceBranchId = request.ServiceBranchId;
        rule.HoursBeforeStart = request.HoursBeforeStart;
        rule.RefundPercentage = request.RefundPercentage;
        rule.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        rule.IsActive = request.IsActive;
        rule.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(rule.Id, cancellationToken);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var rule = await _context.CancellationPolicyRules
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (rule is null)
        {
            return ApiResponse<bool>.Fail("Cancellation policy rule not found.");
        }

        _context.CancellationPolicyRules.Remove(rule);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }

    public async Task<CancellationPolicyQuoteResponse> CalculateCancellationPolicyAsync(
        int serviceId,
        int? serviceBranchId,
        DateTime bookingStartDate,
        decimal paidAmount,
        bool hasSucceededPayment,
        CancellationToken cancellationToken = default)
    {
        if (!hasSucceededPayment || paidAmount <= 0)
        {
            return new CancellationPolicyQuoteResponse
            {
                RefundPercentage = 0,
                RefundAmount = 0,
                RefundRequired = false,
                PolicyDescription = "No succeeded payment found."
            };
        }

        var hoursBeforeStart = (int)Math.Floor((bookingStartDate - DateTime.UtcNow).TotalHours);

        if (hoursBeforeStart < 0)
        {
            hoursBeforeStart = 0;
        }

        var rules = await _context.CancellationPolicyRules
            .AsNoTracking()
            .Where(x =>
                x.IsActive &&
                x.HoursBeforeStart <= hoursBeforeStart &&
                (
                    x.ServiceId == null ||
                    x.ServiceId == serviceId
                ) &&
                (
                    x.ServiceBranchId == null ||
                    x.ServiceBranchId == serviceBranchId
                ))
            .ToListAsync(cancellationToken);

        var selectedRule = rules
            .OrderByDescending(GetSpecificity)
            .ThenByDescending(x => x.HoursBeforeStart)
            .ThenByDescending(x => x.Id)
            .FirstOrDefault();

        if (selectedRule is null)
        {
            return new CancellationPolicyQuoteResponse
            {
                RefundPercentage = 0,
                RefundAmount = 0,
                RefundRequired = false,
                PolicyDescription = "No matching cancellation policy rule found."
            };
        }

        var refundAmount = Math.Round(
            paidAmount * selectedRule.RefundPercentage / 100m,
            2,
            MidpointRounding.AwayFromZero);

        return new CancellationPolicyQuoteResponse
        {
            CancellationPolicyRuleId = selectedRule.Id,
            RefundPercentage = selectedRule.RefundPercentage,
            RefundAmount = refundAmount,
            RefundRequired = refundAmount > 0,
            PolicyDescription = selectedRule.Description
        };
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

    private static int GetSpecificity(CancellationPolicyRule rule)
    {
        if (rule.ServiceId.HasValue && rule.ServiceBranchId.HasValue)
            return 4;

        if (rule.ServiceId.HasValue)
            return 3;

        if (rule.ServiceBranchId.HasValue)
            return 2;

        return 1;
    }

    private static CancellationPolicyRuleResponse MapToResponse(CancellationPolicyRule rule)
    {
        return new CancellationPolicyRuleResponse
        {
            Id = rule.Id,
            ServiceId = rule.ServiceId,
            ServiceName = rule.Service?.Name,
            ServiceBranchId = rule.ServiceBranchId,
            ServiceBranchName = rule.ServiceBranch?.Name,
            HoursBeforeStart = rule.HoursBeforeStart,
            RefundPercentage = rule.RefundPercentage,
            Scope = rule.Scope.ToString(),
            Description = rule.Description,
            IsActive = rule.IsActive,
            CreatedAt = rule.CreatedAt,
            UpdatedAt = rule.UpdatedAt
        };
    }
}