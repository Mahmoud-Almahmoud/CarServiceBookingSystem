using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.NotificationBroadcasts;

namespace CarServiceBookingSystem.Application.Interfaces.INotification;

public interface INotificationBroadcastService
{
    Task<ApiResponse<BroadcastNotificationResponse>> SendAsync(
        BroadcastNotificationRequest request,
        CancellationToken cancellationToken = default);
}