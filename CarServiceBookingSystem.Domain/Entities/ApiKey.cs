namespace CarServiceBookingSystem.Domain.Entities;

public class ApiKey : BaseIdEntity
{
    public string Name { get; set; } = string.Empty;

    public string KeyHash { get; set; } = string.Empty;

    public string? Owner { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? ExpiresAt { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public string? LastUsedIp { get; set; }
}