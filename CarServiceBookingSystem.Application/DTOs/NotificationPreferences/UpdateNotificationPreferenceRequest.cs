namespace CarServiceBookingSystem.Application.DTOs.NotificationPreferences;

public class UpdateNotificationPreferenceRequest
{
    public bool InAppEnabled { get; set; } = true;

    public bool EmailEnabled { get; set; } = false;

    public bool PushEnabled { get; set; } = false;
}