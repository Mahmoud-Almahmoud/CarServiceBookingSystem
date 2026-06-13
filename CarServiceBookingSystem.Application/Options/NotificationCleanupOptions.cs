namespace CarServiceBookingSystem.Application.Options;

public class NotificationCleanupOptions
{
    public bool Enabled { get; set; } = true;

    public int ReadNotificationRetentionDays { get; set; } = 90;

    public bool DeleteOldUnreadNotifications { get; set; } = false;

    public int UnreadNotificationRetentionDays { get; set; } = 180;

    public int BatchSize { get; set; } = 500;
}