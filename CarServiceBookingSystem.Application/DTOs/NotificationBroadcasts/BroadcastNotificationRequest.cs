using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.NotificationBroadcasts;

public class BroadcastNotificationRequest
{
    public NotificationAudienceType AudienceType { get; set; }

    public List<string> UserIds { get; set; } = new();

    public BroadcastNotificationFilterRequest? Filter { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; } = NotificationType.System;

    public NotificationSeverity Severity { get; set; } = NotificationSeverity.Info;

    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    public string? ActionUrl { get; set; }
}