using CarServiceBookingSystem.API.Hubs;
using CarServiceBookingSystem.Application.Constants;
using CarServiceBookingSystem.Application.DTOs.Notifications;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Interfaces.INotification;
using Microsoft.AspNetCore.SignalR;

namespace CarServiceBookingSystem.API.Services;

public class NotificationRealtimeService : INotificationRealtimeService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationRealtimeService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(
        string userId,
        NotificationResponse notification,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.User(userId)
            .SendAsync(
                NotificationHubEvents.NotificationReceived,
                notification,
                cancellationToken);
    }

    public async Task SendUnreadCountAsync(
        string userId,
        int unreadCount,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.User(userId)
            .SendAsync(
                NotificationHubEvents.UnreadNotificationCountChanged,
                unreadCount,
                cancellationToken);
    }

    public async Task SendNotificationReadAsync(
        string userId,
        NotificationReadResponse response,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.User(userId)
            .SendAsync(
                NotificationHubEvents.NotificationRead,
                response,
                cancellationToken);
    }

    public async Task SendNotificationsReadAllAsync(
        string userId,
        NotificationsReadAllResponse response,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.User(userId)
            .SendAsync(
                NotificationHubEvents.NotificationsReadAll,
                response,
                cancellationToken);
    }

    public async Task SendGroupNotificationAsync(
    string groupName,
    NotificationResponse notification,
    CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(groupName)
            .SendAsync(
                NotificationHubEvents.NotificationReceived,
                notification,
                cancellationToken);
    }
}