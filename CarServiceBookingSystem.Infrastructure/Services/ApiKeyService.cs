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
}