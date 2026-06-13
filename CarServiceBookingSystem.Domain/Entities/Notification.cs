using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Domain.Entities;

public class Notification : BaseIdEntity
{

    public string UserId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; }

    public NotificationSeverity Severity { get; set; } = NotificationSeverity.Info;

    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    public string? ActionUrl { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }
}