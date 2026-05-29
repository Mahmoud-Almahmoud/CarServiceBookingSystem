using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Domain.Entities;

public class SecurityAuditLog : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    public SecurityAuditEventType EventType { get; set; }

    public string? IpAddress { get; set; }

    public string? Device { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }

    public string? Details { get; set; }
}