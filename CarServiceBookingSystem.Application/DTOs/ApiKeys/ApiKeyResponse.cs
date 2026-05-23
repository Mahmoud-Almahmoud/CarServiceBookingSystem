namespace CarServiceBookingSystem.Application.DTOs.ApiKeys;

public class ApiKeyResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Owner { get; set; }

    public bool IsActive { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public string? LastUsedIp { get; set; }

    public DateTime CreatedAt { get; set; }
}