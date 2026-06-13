using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.Notifications;

public class CreateNotificationRequest
{
    public string UserId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; }

    public NotificationSeverity Severity { get; set; } = NotificationSeverity.Info;

    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    public string? ActionUrl { get; set; }
}