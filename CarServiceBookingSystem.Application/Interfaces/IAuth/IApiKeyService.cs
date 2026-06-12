using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.ApiKeys;

namespace CarServiceBookingSystem.Application.Interfaces.IAuth;

public interface IApiKeyService
{
    Task<ApiResponse<ApiKeyCreatedResponse>> CreateAsync(string name, string? owner, DateTime? expiresAt);
    Task<bool> ValidateAsync(string rawKey, string? ipAddress);
    Task<ApiResponse<string>> RevokeAsync(int id);
    Task<ApiResponse<List<ApiKeyResponse>>> GetAllAsync();
    Task<ApiResponse<List<ApiKeyUsageDto>>> GetUsageAsync(ApiKeyUsageQuery query);
    Task<ApiResponse<ApiKeyUsageDto>> GetUsageByIdAsync(int apiKeyId);
    Task<ApiResponse<ApiKeyUsageSummaryDto>> GetUsageSummaryAsync();
    Task<ApiResponse<List<StaleApiKeyDto>>> GetStaleKeysAsync(int days = 30);
}