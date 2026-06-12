namespace CarServiceBookingSystem.Application.DTOs.Notifications;

public class NotificationReadResponse
{
    public int NotificationId { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }
}