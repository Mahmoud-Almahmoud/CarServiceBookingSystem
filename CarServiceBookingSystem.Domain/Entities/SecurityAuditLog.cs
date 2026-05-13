namespace CarServiceBookingSystem.Domain.Entities;

public class SecurityAuditLog : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    public string EventType { get; set; } = string.Empty;

    public string? IpAddress { get; set; }

    public string? Device { get; set; }

    public string? Details { get; set; }
}