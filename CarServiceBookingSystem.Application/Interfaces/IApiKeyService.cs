using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.ApiKeys;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface IApiKeyService
{
    Task<ApiResponse<ApiKeyCreatedResponse>> CreateAsync(string name, string? owner, DateTime? expiresAt);
    Task<bool> ValidateAsync(string rawKey, string? ipAddress);
    Task<ApiResponse<string>> RevokeAsync(int id);
    Task<ApiResponse<List<ApiKeyResponse>>> GetAllAsync();
}