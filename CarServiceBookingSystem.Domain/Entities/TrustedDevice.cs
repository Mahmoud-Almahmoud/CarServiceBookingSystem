namespace CarServiceBookingSystem.Domain.Entities;

public class TrustedDevice : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    public string TokenHash { get; set; } = string.Empty;

    public string? DeviceName { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }

}