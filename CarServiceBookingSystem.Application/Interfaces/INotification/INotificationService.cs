using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Notifications;

namespace CarServiceBookingSystem.Application.Interfaces.INotification;


public interface INotificationService
{
Task<ApiResponse<NotificationResponse>> CreateAsync(
    CreateNotificationRequest request,
    CancellationToken cancellationToken = default);

Task<ApiResponse<List<NotificationResponse>>> CreateManyAsync(
    List<CreateNotificationRequest> requests,
    CancellationToken cancellationToken = default);

Task<ApiResponse<PagedResponse<NotificationResponse>>> GetMyNotificationsAsync(
    NotificationQueryRequest request,
    CancellationToken cancellationToken = default);

Task<ApiResponse<UnreadNotificationCountResponse>> GetMyUnreadCountAsync(
    CancellationToken cancellationToken = default);

Task<ApiResponse<NotificationReadResponse>> MarkAsReadAsync(
    int notificationId,
    CancellationToken cancellationToken = default);

Task<ApiResponse<NotificationsReadAllResponse>> MarkAllAsReadAsync(
    CancellationToken cancellationToken = default);

Task<ApiResponse<bool>> DeleteAsync(
    int notificationId,
    CancellationToken cancellationToken = default);
}