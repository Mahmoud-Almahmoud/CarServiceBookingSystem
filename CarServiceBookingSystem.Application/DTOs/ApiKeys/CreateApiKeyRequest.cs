namespace CarServiceBookingSystem.Application.DTOs.ApiKeys;

public class CreateApiKeyRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Owner { get; set; }
    public DateTime? ExpiresAt { get; set; }
}