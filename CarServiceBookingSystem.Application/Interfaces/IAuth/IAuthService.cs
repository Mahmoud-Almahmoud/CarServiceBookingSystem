using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Auth;

namespace CarServiceBookingSystem.Application.Interfaces.IAuth;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<ApiResponse<string>> LogoutAsync(LogoutRequest request);
    Task<ApiResponse<string>> LogoutAllDevicesAsync();
    Task<ApiResponse<List<ActiveSessionResponse>>> GetActiveSessionsAsync();
    Task<ApiResponse<string>> RevokeSessionAsync(int sessionId);
    Task<ApiResponse<string>> ConfirmEmailAsync(string userId, string token);
    Task<ApiResponse<string>> ResendEmailConfirmationAsync(ResendEmailConfirmationRequest request);
    Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequest request);
    Task<ApiResponse<string>> ChangePasswordAsync(ChangePasswordRequest request);
    Task<ApiResponse<EnableTwoFactorResponse>> GetTwoFactorSetupAsync();
    Task<ApiResponse<string>> EnableTwoFactorAsync(VerifyTwoFactorRequest request);
    Task<ApiResponse<AuthResponse>> LoginWithTwoFactorAsync(LoginTwoFactorRequest request);
    Task<ApiResponse<string>> DisableTwoFactorAsync(DisableTwoFactorRequest request);
    Task<ApiResponse<RecoveryCodesResponse>> GenerateRecoveryCodesAsync();
    Task<ApiResponse<AuthResponse>> LoginWithRecoveryCodeAsync(LoginRecoveryCodeRequest request);
    Task<ApiResponse<byte[]>> GetTwoFactorQrCodeAsync();
    Task<ApiResponse<List<TrustedDeviceResponse>>> GetTrustedDevicesAsync();
    Task<ApiResponse<string>> RevokeTrustedDeviceAsync(int deviceId);
    Task<ApiResponse<string>> RevokeAllTrustedDevicesAsync();
}