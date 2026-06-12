using CarServiceBookingSystem.Application.DTOs.Notifications;

namespace CarServiceBookingSystem.Application.Interfaces.INotification;

public interface INotificationRealtimeService
{
    Task SendNotificationAsync(
        string userId,
        NotificationResponse notification,
        CancellationToken cancellationToken = default);

    Task SendUnreadCountAsync(
        string userId,
        int unreadCount,
        CancellationToken cancellationToken = default);

    Task SendNotificationReadAsync(
        string userId,
        NotificationReadResponse response,
        CancellationToken cancellationToken = default);

    Task SendNotificationsReadAllAsync(
        string userId,
        NotificationsReadAllResponse response,
        CancellationToken cancellationToken = default);
}