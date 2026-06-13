using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.NotificationPreferences;

public class BulkUpdateNotificationPreferencesRequest
{
    public List<NotificationPreferenceItemRequest> Preferences { get; set; } = new();
}

public class NotificationPreferenceItemRequest
{
    public NotificationType Type { get; set; }

    public bool InAppEnabled { get; set; } = true;

    public bool EmailEnabled { get; set; } = false;

    public bool PushEnabled { get; set; } = false;
}