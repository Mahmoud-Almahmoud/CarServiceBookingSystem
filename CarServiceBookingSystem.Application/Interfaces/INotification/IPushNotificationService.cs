using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Notifications;
using CarServiceBookingSystem.Application.DTOs.PushNotifications;

namespace CarServiceBookingSystem.Application.Interfaces.INotification;

public interface IPushNotificationService
{
    Task<ApiResponse<WebPushPublicKeyResponse>> GetPublicKeyAsync(
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PushSubscriptionResponse>> SubscribeAsync(
        PushSubscriptionRequest request,
        string? userAgent,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> UnsubscribeAsync(
        string endpoint,
        CancellationToken cancellationToken = default);

    Task SendToUserAsync(
        string userId,
        NotificationResponse notification,
        CancellationToken cancellationToken = default);
}