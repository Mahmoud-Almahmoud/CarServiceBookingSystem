namespace CarServiceBookingSystem.Application.DTOs.Auth;

public class AuthResponse
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int SessionId { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
}