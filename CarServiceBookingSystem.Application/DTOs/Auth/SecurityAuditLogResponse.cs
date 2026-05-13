namespace CarServiceBookingSystem.Application.DTOs.Auth;

public class SecurityAuditLogResponse
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? Device { get; set; }
    public string? Details { get; set; }
    public DateTime CreatedAt { get; set; }
}