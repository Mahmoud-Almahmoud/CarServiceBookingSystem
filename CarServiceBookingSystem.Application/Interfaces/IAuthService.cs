using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Auth;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<ApiResponse<string>> LogoutAsync(LogoutRequest request);
}