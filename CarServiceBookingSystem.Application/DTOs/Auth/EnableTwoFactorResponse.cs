namespace CarServiceBookingSystem.Application.DTOs.Auth;

public class EnableTwoFactorResponse
{
    public string SharedKey { get; set; } = string.Empty;
    public string AuthenticatorUri { get; set; } = string.Empty;
}