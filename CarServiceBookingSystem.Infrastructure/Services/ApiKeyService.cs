using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.ApiKeys;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Authentication;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly ApplicationDbContext _context;

    public ApiKeyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<ApiKeyCreatedResponse>> CreateAsync(
        string name,
        string? owner,
        DateTime? expiresAt)
    {
        var rawKey = $"csbs_{Convert.ToBase64String(RandomNumberGenerator.GetBytes(48))}";
        var keyHash = TokenHasher.Hash(rawKey);

        var apiKey = new ApiKey
        {
            Name = name,
            Owner = owner,
            KeyHash = keyHash,
            ExpiresAt = expiresAt,
            IsActive = true
        };

        await _context.ApiKeys.AddAsync(apiKey);
        await _context.SaveChangesAsync();

        return ApiResponse<ApiKeyCreatedResponse>.Ok(new ApiKeyCreatedResponse
        {
            Id = apiKey.Id,
            Name = apiKey.Name,
            RawKey = rawKey
        });
    }

    public async Task<bool> ValidateAsync(string rawKey, string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(rawKey))
            return false;

        var keyHash = TokenHasher.Hash(rawKey);

        var apiKey = await _context.ApiKeys
            .FirstOrDefaultAsync(x =>
                x.KeyHash == keyHash &&
                x.IsActive &&
                (x.ExpiresAt == null || x.ExpiresAt > DateTime.UtcNow));

        if (apiKey == null)
            return false;

        apiKey.LastUsedAt = DateTime.UtcNow;
        apiKey.LastUsedIp = ipAddress;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<ApiResponse<string>> RevokeAsync(int id)
    {
        var apiKey = await _context.ApiKeys.FindAsync(id);

        if (apiKey == null)
            return ApiResponse<string>.Fail("API key not found");

        apiKey.IsActive = false;

        await _context.SaveChangesAsync();

        return ApiResponse<string>.Ok("API key revoked successfully");
    }

    public async Task<ApiResponse<List<ApiKeyResponse>>> GetAllAsync()
    {
        var keys = await _context.ApiKeys
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ApiKeyResponse
            {
                Id = x.Id,
                Name = x.Name,
                Owner = x.Owner,
                IsActive = x.IsActive,
                ExpiresAt = x.ExpiresAt,
                LastUsedAt = x.LastUsedAt,
                LastUsedIp = x.LastUsedIp,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return ApiResponse<List<ApiKeyResponse>>.Ok(keys);
    }

    public async Task<ApiResponse<List<ApiKeyUsageDto>>> GetUsageAsync(ApiKeyUsageQuery request)
    {
        var now = DateTime.UtcNow;

        var query = _context.ApiKeys
            .AsNoTracking()
            .AsQueryable();

        if (request.ActiveOnly == true)
        {
            query = query.Where(x => x.IsActive);
        }

        if (request.UnusedOnly == true)
        {
            query = query.Where(x => x.LastUsedAt == null);
        }

        if (request.ExpiredOnly == true)
        {
            query = query.Where(x => x.ExpiresAt != null && x.ExpiresAt <= now);
        }

        var data = await query
            .OrderByDescending(x => x.LastUsedAt ?? x.CreatedAt)
            .Select(x => new ApiKeyUsageDto
            {
                Id = x.Id,
                Name = x.Name,
                CreatedAt = x.CreatedAt,
                LastUsedAt = x.LastUsedAt,
                LastUsedIpAddress = x.LastUsedIp,
                IsActive = x.IsActive,
                ExpiresAt = x.ExpiresAt,
                IsExpired = x.ExpiresAt != null && x.ExpiresAt <= now
            })
            .ToListAsync();

        return ApiResponse<List<ApiKeyUsageDto>>.Ok(data);
    }
    public async Task<ApiResponse<ApiKeyUsageDto>> GetUsageByIdAsync(int apiKeyId)
    {
        var now = DateTime.UtcNow;

        var apiKey = await _context.ApiKeys
            .AsNoTracking()
            .Where(x => x.Id == apiKeyId)
            .Select(x => new ApiKeyUsageDto
            {
                Id = x.Id,
                Name = x.Name,
                CreatedAt = x.CreatedAt,
                LastUsedAt = x.LastUsedAt,
                LastUsedIpAddress = x.LastUsedIp,
                IsActive = x.IsActive,
                ExpiresAt = x.ExpiresAt,
                IsExpired = x.ExpiresAt != null && x.ExpiresAt <= now
            })
            .FirstOrDefaultAsync();

        if (apiKey is null)
            return ApiResponse<ApiKeyUsageDto>.Fail("API key not found.");

        return ApiResponse<ApiKeyUsageDto>.Ok(apiKey);
    }

    public async Task<ApiResponse<ApiKeyUsageSummaryDto>> GetUsageSummaryAsync()
    {
        var now = DateTime.UtcNow;

        var summary = new ApiKeyUsageSummaryDto
        {
            TotalKeys = await _context.ApiKeys.CountAsync(),
            ActiveKeys = await _context.ApiKeys.CountAsync(x => x.IsActive),
            RevokedKeys = await _context.ApiKeys.CountAsync(x => !x.IsActive),
            ExpiredKeys = await _context.ApiKeys.CountAsync(x => x.ExpiresAt != null && x.ExpiresAt <= now),
            UnusedKeys = await _context.ApiKeys.CountAsync(x => x.LastUsedAt == null),
            MostRecentUsageAt = await _context.ApiKeys
                .Where(x => x.LastUsedAt != null)
                .MaxAsync(x => x.LastUsedAt)
        };

        return ApiResponse<ApiKeyUsageSummaryDto>.Ok(summary);
    }

    public async Task<ApiResponse<List<StaleApiKeyDto>>> GetStaleKeysAsync(int days = 30)
    {
        if (days <= 0)
            return ApiResponse<List<StaleApiKeyDto>>.Fail("Days must be greater than zero.");

        var now = DateTime.UtcNow;
        var cutoff = now.AddDays(-days);

        var keys = await _context.ApiKeys
            .AsNoTracking()
            .Where(x =>
                x.IsActive &&
                (
                    x.LastUsedAt == null ||
                    x.LastUsedAt <= cutoff
                ))
            .OrderBy(x => x.LastUsedAt ?? x.CreatedAt)
            .Select(x => new StaleApiKeyDto
            {
                Id = x.Id,
                Name = x.Name,
                CreatedAt = x.CreatedAt,
                LastUsedAt = x.LastUsedAt,
                ExpiresAt = x.ExpiresAt,
                DaysSinceLastUse = x.LastUsedAt == null
                    ? EF.Functions.DateDiffDay(x.CreatedAt, now)
                    : EF.Functions.DateDiffDay(x.LastUsedAt.Value, now)
            })
            .ToListAsync();

        return ApiResponse<List<StaleApiKeyDto>>.Ok(keys);
    }
}