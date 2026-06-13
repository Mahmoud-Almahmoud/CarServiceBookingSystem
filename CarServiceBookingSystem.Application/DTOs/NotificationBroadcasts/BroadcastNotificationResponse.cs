namespace CarServiceBookingSystem.Application.DTOs.NotificationBroadcasts;

public class BroadcastNotificationResponse
{
    public int TargetUserCount { get; set; }

    public int CreatedNotificationCount { get; set; }

    public int BatchCount { get; set; }
}