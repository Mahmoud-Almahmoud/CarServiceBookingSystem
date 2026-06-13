using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.NotificationPreferences;

public class NotificationDeliveryPreferenceResponse
{
    public NotificationType Type { get; set; }

    public bool InAppEnabled { get; set; }

    public bool PushEnabled { get; set; }

    public bool EmailEnabled { get; set; }

    public bool IsMandatory { get; set; }
}