using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Auth;
using CarServiceBookingSystem.Application.DTOs.Security;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Enums;
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

    public async Task<ApiResponse<PagedResponse<SecurityAuditLogResponse>>> GetLogsAsync(SecurityAuditLogRequest request)
    {
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 20 : request.PageSize;
        request.PageSize = request.PageSize > 100 ? 100 : request.PageSize;

        var query = _context.SecurityAuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.UserId))
            query = query.Where(x => x.UserId == request.UserId);

        if (request.EventType.HasValue)
            query = query.Where(x => x.EventType == request.EventType.Value);

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
                EventType = x.EventType.ToString(),
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

    public async Task<ApiResponse<PagedResponse<SecurityAuditLogResponse>>> GetMyLogsAsync(string userId, MyActivityRequest request)
    {
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 20 : request.PageSize;
        request.PageSize = request.PageSize > 100 ? 100 : request.PageSize;

        var query = _context.SecurityAuditLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        if (request.EventType.HasValue)
            query = query.Where(x => x.EventType == request.EventType.Value);

        if (request.FromUtc.HasValue)
            query = query.Where(x => x.CreatedAt >= request.FromUtc.Value);

        if (request.ToUtc.HasValue)
            query = query.Where(x => x.CreatedAt <= request.ToUtc.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.EventType.ToString().Contains(search) ||
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
                EventType = x.EventType.ToString(),
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

    public async Task<ApiResponse<List<SecurityAuditSummaryDto>>> GetSummaryAsync(int days)
    {
        if (days <= 0 || days > 365)
            return ApiResponse<List<SecurityAuditSummaryDto>>.Fail("Days must be between 1 and 365.");

        var toUtc = DateTime.UtcNow;
        var fromUtc = toUtc.AddDays(-days);

        var result = await _context.SecurityAuditLogs
        .AsNoTracking()
        .Where(x => x.CreatedAt >= fromUtc)
        .GroupBy(x => x.EventType)
        .Select(g => new SecurityAuditSummaryDto
        {
            EventType = g.Key,
            Count = g.Count(),
            FromUtc = fromUtc,
            ToUtc = toUtc
        })
        .OrderByDescending(x => x.Count)
        .ToListAsync();

        return ApiResponse<List<SecurityAuditSummaryDto>>.Ok(result);
    }

    public async Task<ApiResponse<List<SecurityAuditTopCountryDto>>> GetTopCountriesAsync(
    int days,
    int take)
    {
        if (days <= 0 || days > 365)
            return ApiResponse<List<SecurityAuditTopCountryDto>>.Fail("Days must be between 1 and 365.");

        if (take <= 0 || take > 50)
            return ApiResponse<List<SecurityAuditTopCountryDto>>.Fail("Take must be between 1 and 50.");

        var fromUtc = DateTime.UtcNow.AddDays(-days);

        var result = await _context.SecurityAuditLogs
            .AsNoTracking()
            .Where(x => x.CreatedAt >= fromUtc && x.Country != null && x.Country != "")
            .GroupBy(x => x.Country!)
            .Select(g => new SecurityAuditTopCountryDto
            {
                Country = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(take)
            .ToListAsync();

        return ApiResponse<List<SecurityAuditTopCountryDto>>.Ok(result);
    }

    public async Task<ApiResponse<List<SecurityAuditTopUserDto>>> GetTopUsersAsync(int days, int take)
    {
        if (days <= 0 || days > 365)
            return ApiResponse<List<SecurityAuditTopUserDto>>.Fail("Days must be between 1 and 365.");

        if (take <= 0 || take > 50)
            return ApiResponse<List<SecurityAuditTopUserDto>>.Fail("Take must be between 1 and 50.");

        var fromUtc = DateTime.UtcNow.AddDays(-days);

        var groupedLogs = _context.SecurityAuditLogs
            .AsNoTracking()
            .Where(x => x.CreatedAt >= fromUtc && x.UserId != null)
            .GroupBy(x => x.UserId!)
            .Select(g => new
            {
                UserId = g.Key,
                EventCount = g.Count()
            });

        var result = await groupedLogs
            .Join(
                _context.Users.AsNoTracking(),
                log => log.UserId,
                user => user.Id,
                (log, user) => new SecurityAuditTopUserDto
                {
                    UserId = log.UserId,
                    UserName = user.UserName,
                    Email = user.Email,
                    EventCount = log.EventCount
                })
            .OrderByDescending(x => x.EventCount)
            .Take(take)
            .ToListAsync();

        return ApiResponse<List<SecurityAuditTopUserDto>>.Ok(result);
    }

    public async Task<ApiResponse<List<FailedLoginAnalyticsDto>>> GetFailedLoginsAsync(
    int days,
    int take)
    {
        if (days <= 0 || days > 365)
            return ApiResponse<List<FailedLoginAnalyticsDto>>.Fail("Days must be between 1 and 365.");

        if (take <= 0 || take > 50)
            return ApiResponse<List<FailedLoginAnalyticsDto>>.Fail("Take must be between 1 and 50.");

        var fromUtc = DateTime.UtcNow.AddDays(-days);

        var result = await _context.SecurityAuditLogs
            .AsNoTracking()
            .Where(x =>
                x.CreatedAt >= fromUtc &&
                x.EventType == SecurityAuditEventType.LoginFailed)
            .GroupBy(x => new
            {
                x.UserId,
                x.IpAddress,
                x.Country
            })
            .Select(g => new FailedLoginAnalyticsDto
            {
                UserId = g.Key.UserId,
                IpAddress = g.Key.IpAddress,
                Country = g.Key.Country,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(take)
            .ToListAsync();

        return ApiResponse<List<FailedLoginAnalyticsDto>>.Ok(result);
    }

    public async Task<ApiResponse<List<SuspiciousLoginAnalyticsDto>>> GetSuspiciousLoginsAsync(
    int days,
    int take)
    {
        if (days <= 0 || days > 365)
            return ApiResponse<List<SuspiciousLoginAnalyticsDto>>.Fail("Days must be between 1 and 365.");

        if (take <= 0 || take > 50)
            return ApiResponse<List<SuspiciousLoginAnalyticsDto>>.Fail("Take must be between 1 and 50.");

        var fromUtc = DateTime.UtcNow.AddDays(-days);

        var groupedLogs = _context.SecurityAuditLogs
            .AsNoTracking()
            .Where(x =>
                x.CreatedAt >= fromUtc &&
                x.EventType == SecurityAuditEventType.SuspiciousLogin)
            .GroupBy(x => new
            {
                x.UserId,
                x.IpAddress,
                x.Country,
                x.City
            })
            .Select(g => new
            {
                g.Key.UserId,
                g.Key.IpAddress,
                g.Key.Country,
                g.Key.City,
                Count = g.Count(),
                LastOccurredAt = g.Max(x => x.CreatedAt)
            });

        var result = await groupedLogs
            .GroupJoin(
                _context.Users.AsNoTracking(),
                log => log.UserId,
                user => user.Id,
                (log, users) => new { log, users })
            .SelectMany(
                x => x.users.DefaultIfEmpty(),
                (x, user) => new SuspiciousLoginAnalyticsDto
                {
                    UserId = x.log.UserId,
                    Email = user != null ? user.Email : null,
                    IpAddress = x.log.IpAddress,
                    Country = x.log.Country,
                    City = x.log.City,
                    Count = x.log.Count,
                    LastOccurredAt = x.log.LastOccurredAt
                })
            .OrderByDescending(x => x.Count)
            .ThenByDescending(x => x.LastOccurredAt)
            .Take(take)
            .ToListAsync();

        return ApiResponse<List<SuspiciousLoginAnalyticsDto>>.Ok(result);
    }

    public async Task<ApiResponse<List<RiskyIpAnalyticsDto>>> GetRiskyIpsAsync(
    int days,
    int take)
    {
        if (days <= 0 || days > 365)
            return ApiResponse<List<RiskyIpAnalyticsDto>>.Fail("Days must be between 1 and 365.");

        if (take <= 0 || take > 50)
            return ApiResponse<List<RiskyIpAnalyticsDto>>.Fail("Take must be between 1 and 50.");

        var fromUtc = DateTime.UtcNow.AddDays(-days);

        var riskyEvents = new[]
        {
        SecurityAuditEventType.LoginFailed,
        SecurityAuditEventType.SuspiciousLogin,
        SecurityAuditEventType.AccessDenied
    };

        var result = await _context.SecurityAuditLogs
            .AsNoTracking()
            .Where(x =>
                x.CreatedAt >= fromUtc &&
                x.IpAddress != null &&
                riskyEvents.Contains(x.EventType))
            .GroupBy(x => new
            {
                x.IpAddress,
                x.Country,
                x.City
            })
            .Select(g => new RiskyIpAnalyticsDto
            {
                IpAddress = g.Key.IpAddress!,
                Country = g.Key.Country,
                City = g.Key.City,

                FailedLogins = g.Count(x => x.EventType == SecurityAuditEventType.LoginFailed),
                SuspiciousLogins = g.Count(x => x.EventType == SecurityAuditEventType.SuspiciousLogin),
                AccessDeniedEvents = g.Count(x => x.EventType == SecurityAuditEventType.AccessDenied),
                TotalRiskEvents = g.Count(),

                LastOccurredAt = g.Max(x => x.CreatedAt)
            })
            .OrderByDescending(x => x.TotalRiskEvents)
            .ThenByDescending(x => x.LastOccurredAt)
            .Take(take)
            .ToListAsync();

        return ApiResponse<List<RiskyIpAnalyticsDto>>.Ok(result);
    }

    public async Task<ApiResponse<List<AccessDeniedAnalyticsDto>>> GetAccessDeniedAsync(
    int days,
    int take)
    {
        if (days <= 0 || days > 365)
            return ApiResponse<List<AccessDeniedAnalyticsDto>>.Fail("Days must be between 1 and 365.");

        if (take <= 0 || take > 50)
            return ApiResponse<List<AccessDeniedAnalyticsDto>>.Fail("Take must be between 1 and 50.");

        var fromUtc = DateTime.UtcNow.AddDays(-days);

        var groupedLogs = _context.SecurityAuditLogs
            .AsNoTracking()
            .Where(x =>
                x.CreatedAt >= fromUtc &&
                x.EventType == SecurityAuditEventType.AccessDenied)
            .GroupBy(x => new
            {
                x.UserId,
                x.IpAddress,
                x.Country
            })
            .Select(g => new
            {
                g.Key.UserId,
                g.Key.IpAddress,
                g.Key.Country,
                Count = g.Count(),
                LastOccurredAt = g.Max(x => x.CreatedAt)
            });

        var result = await groupedLogs
            .GroupJoin(
                _context.Users.AsNoTracking(),
                log => log.UserId,
                user => user.Id,
                (log, users) => new { log, users })
            .SelectMany(
                x => x.users.DefaultIfEmpty(),
                (x, user) => new AccessDeniedAnalyticsDto
                {
                    UserId = x.log.UserId,
                    Email = user != null ? user.Email : null,
                    IpAddress = x.log.IpAddress,
                    Country = x.log.Country,
                    Count = x.log.Count,
                    LastOccurredAt = x.log.LastOccurredAt
                })
            .OrderByDescending(x => x.Count)
            .ThenByDescending(x => x.LastOccurredAt)
            .Take(take)
            .ToListAsync();

        return ApiResponse<List<AccessDeniedAnalyticsDto>>.Ok(result);
    }

    public async Task<ApiResponse<List<SecurityAuditDailyTrendDto>>> GetDailyTrendAsync(int days)
    {
        if (days <= 0 || days > 365)
            return ApiResponse<List<SecurityAuditDailyTrendDto>>.Fail("Days must be between 1 and 365.");

        var fromUtc = DateTime.UtcNow.Date.AddDays(-days + 1);

        var result = await _context.SecurityAuditLogs
            .AsNoTracking()
            .Where(x => x.CreatedAt >= fromUtc)
            .GroupBy(x => x.CreatedAt.Date)
            .Select(g => new SecurityAuditDailyTrendDto
            {
                Date = g.Key,
                TotalEvents = g.Count(),
                LoginSucceeded = g.Count(x => x.EventType == SecurityAuditEventType.LoginSucceeded),
                LoginFailed = g.Count(x => x.EventType == SecurityAuditEventType.LoginFailed),
                SuspiciousLogins = g.Count(x => x.EventType == SecurityAuditEventType.SuspiciousLogin),
                AccessDenied = g.Count(x => x.EventType == SecurityAuditEventType.AccessDenied)
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        return ApiResponse<List<SecurityAuditDailyTrendDto>>.Ok(result);
    }

    public async Task<ApiResponse<List<SecurityAuditHourlyRiskTrendDto>>> GetHourlyRiskTrendAsync(int hours)
    {
        if (hours <= 0 || hours > 168)
            return ApiResponse<List<SecurityAuditHourlyRiskTrendDto>>.Fail("Hours must be between 1 and 168.");

        var toUtc = DateTime.UtcNow;
        var fromUtc = toUtc.AddHours(-hours);

        var riskEvents = new[]
        {
        SecurityAuditEventType.LoginFailed,
        SecurityAuditEventType.SuspiciousLogin,
        SecurityAuditEventType.AccessDenied,
        SecurityAuditEventType.ApiKeyValidationFailed
    };

        var result = await _context.SecurityAuditLogs
            .AsNoTracking()
            .Where(x =>
                x.CreatedAt >= fromUtc &&
                riskEvents.Contains(x.EventType))
            .GroupBy(x => new
            {
                x.CreatedAt.Year,
                x.CreatedAt.Month,
                x.CreatedAt.Day,
                x.CreatedAt.Hour
            })
            .Select(g => new SecurityAuditHourlyRiskTrendDto
            {
                HourUtc = new DateTime(
                    g.Key.Year,
                    g.Key.Month,
                    g.Key.Day,
                    g.Key.Hour,
                    0,
                    0,
                    DateTimeKind.Utc),

                FailedLogins = g.Count(x => x.EventType == SecurityAuditEventType.LoginFailed),
                SuspiciousLogins = g.Count(x => x.EventType == SecurityAuditEventType.SuspiciousLogin),
                AccessDenied = g.Count(x => x.EventType == SecurityAuditEventType.AccessDenied),
                ApiKeyValidationFailed = g.Count(x => x.EventType == SecurityAuditEventType.ApiKeyValidationFailed),

                TotalRiskEvents = g.Count()
            })
            .OrderBy(x => x.HourUtc)
            .ToListAsync();

        return ApiResponse<List<SecurityAuditHourlyRiskTrendDto>>.Ok(result);
    }

    public async Task<ApiResponse<SecurityScoreDto>> GetSecurityScoreAsync()
    {
        var now = DateTime.UtcNow;
        var fromUtc = now.AddHours(-24);

        var failedLogins = await _context.SecurityAuditLogs.CountAsync(x =>
            x.CreatedAt >= fromUtc &&
            x.EventType == SecurityAuditEventType.LoginFailed);

        var suspiciousLogins = await _context.SecurityAuditLogs.CountAsync(x =>
            x.CreatedAt >= fromUtc &&
            x.EventType == SecurityAuditEventType.SuspiciousLogin);

        var accessDenied = await _context.SecurityAuditLogs.CountAsync(x =>
            x.CreatedAt >= fromUtc &&
            x.EventType == SecurityAuditEventType.AccessDenied);

        var apiKeyValidationFailed = await _context.SecurityAuditLogs.CountAsync(x =>
            x.CreatedAt >= fromUtc &&
            x.EventType == SecurityAuditEventType.ApiKeyValidationFailed);

        var activeApiKeys = await _context.ApiKeys.CountAsync(x => x.IsActive);

        var expiredApiKeys = await _context.ApiKeys.CountAsync(x =>
            x.ExpiresAt != null && x.ExpiresAt <= now);

        var totalUsers = await _context.Users.CountAsync();

        var usersWithTwoFactorEnabled = await _context.Users.CountAsync(x =>
            x.TwoFactorEnabled);

        var twoFactorRate = totalUsers == 0
            ? 0
            : Math.Round((decimal)usersWithTwoFactorEnabled / totalUsers * 100, 2);

        var score = 100;

        score -= Math.Min(failedLogins, 20);
        score -= suspiciousLogins * 5;
        score -= accessDenied * 2;
        score -= apiKeyValidationFailed * 3;
        score -= expiredApiKeys * 2;

        if (twoFactorRate >= 80)
            score += 10;
        else if (twoFactorRate < 30)
            score -= 10;

        score = Math.Clamp(score, 0, 100);

        var dto = new SecurityScoreDto
        {
            Score = score,

            FailedLoginsLast24Hours = failedLogins,
            SuspiciousLoginsLast24Hours = suspiciousLogins,
            AccessDeniedLast24Hours = accessDenied,
            ApiKeyValidationFailedLast24Hours = apiKeyValidationFailed,

            ActiveApiKeys = activeApiKeys,
            ExpiredApiKeys = expiredApiKeys,

            TotalUsers = totalUsers,
            UsersWithTwoFactorEnabled = usersWithTwoFactorEnabled,
            TwoFactorAdoptionRate = twoFactorRate
        };

        return ApiResponse<SecurityScoreDto>.Ok(dto);
    }
}