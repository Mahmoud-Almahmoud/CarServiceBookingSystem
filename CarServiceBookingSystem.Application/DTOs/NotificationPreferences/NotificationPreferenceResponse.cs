using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.DTOs.NotificationPreferences;

public class NotificationPreferenceResponse
{
    public NotificationType Type { get; set; }

    public string TypeName { get; set; } = string.Empty;

    public bool InAppEnabled { get; set; }

    public bool EmailEnabled { get; set; }

    public bool PushEnabled { get; set; }

    public bool IsMandatory { get; set; }

    public DateTime? UpdatedAt { get; set; }
}