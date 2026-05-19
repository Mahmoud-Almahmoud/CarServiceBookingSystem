namespace CarServiceBookingSystem.Application.DTOs.Auth;

public class TrustedDeviceResponse
{
    public int Id { get; set; }

    public string? DeviceName { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }
}