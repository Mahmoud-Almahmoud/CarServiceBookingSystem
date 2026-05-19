namespace CarServiceBookingSystem.Application.DTOs.Auth;

public class VerifyTwoFactorRequest
{
    public string Code { get; set; } = string.Empty;
}