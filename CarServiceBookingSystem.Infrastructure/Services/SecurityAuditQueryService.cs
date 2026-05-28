using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Auth;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class SecurityAuditQueryService : ISecurityAuditQueryService
{
    private readonly ApplicationDbContext _context;

    public SecurityAuditQueryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PagedResponse<SecurityAuditLogResponse>>> GetLogsAsync(
        SecurityAuditLogRequest request)
    {
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 20 : request.PageSize;
        request.PageSize = request.PageSize > 100 ? 100 : request.PageSize;

        var query = _context.SecurityAuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.UserId))
            query = query.Where(x => x.UserId == request.UserId);

        if (!string.IsNullOrWhiteSpace(request.EventType))
            query = query.Where(x => x.EventType == request.EventType);

        if (!string.IsNullOrWhiteSpace(request.IpAddress))
            query = query.Where(x => x.IpAddress == request.IpAddress);

        if (!string.IsNullOrWhiteSpace(request.Country))
            query = query.Where(x => x.Country == request.Country);

        if (request.FromUtc.HasValue)
            query = query.Where(x => x.CreatedAt >= request.FromUtc.Value);

        if (request.ToUtc.HasValue)
            query = query.Where(x => x.CreatedAt <= request.ToUtc.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.UserId.Contains(search) ||
                x.EventType.Contains(search) ||
                (x.IpAddress != null && x.IpAddress.Contains(search)));
        }

        query = request.SortBy?.ToLower() switch
        {
            "eventtype" => request.Desc
                ? query.OrderByDescending(x => x.EventType).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.EventType).ThenBy(x => x.Id),

            "createdat" => request.Desc
                ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.Id)
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new SecurityAuditLogResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                EventType = x.EventType,
                IpAddress = x.IpAddress,
                Device = x.Device,
                Details = x.Details,
                CreatedAt = x.CreatedAt,
                Country = x.Country,
                City = x.City
            })
            .ToListAsync();

        return ApiResponse<PagedResponse<SecurityAuditLogResponse>>.Ok(
            new PagedResponse<SecurityAuditLogResponse>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
    }

    public async Task<ApiResponse<PagedResponse<SecurityAuditLogResponse>>> GetMyLogsAsync(string userId,PagedRequest request)
    {
        var query = _context.SecurityAuditLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.EventType.Contains(search) ||
                (x.IpAddress != null && x.IpAddress.Contains(search)));
        }

        query = query.OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new SecurityAuditLogResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                EventType = x.EventType,
                IpAddress = x.IpAddress,
                Device = x.Device,
                Details = x.Details,
                CreatedAt = x.CreatedAt,
                Country = x.Country,
                City = x.City
            })
            .ToListAsync();

        return ApiResponse<PagedResponse<SecurityAuditLogResponse>>.Ok(
            new PagedResponse<SecurityAuditLogResponse>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
    }

    
}