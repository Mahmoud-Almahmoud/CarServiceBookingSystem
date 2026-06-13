using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.NotificationPreferences;
using CarServiceBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.Interfaces.INotification
{
    public interface INotificationPreferenceService
    {
        Task<ApiResponse<List<NotificationPreferenceResponse>>> GetMyPreferencesAsync(
            CancellationToken cancellationToken = default);

        Task<ApiResponse<NotificationPreferenceResponse>> UpdateMyPreferenceAsync(
            NotificationType type,
            UpdateNotificationPreferenceRequest request,
            CancellationToken cancellationToken = default);

        Task<ApiResponse<List<NotificationPreferenceResponse>>> BulkUpdateMyPreferencesAsync(
            BulkUpdateNotificationPreferencesRequest request,
            CancellationToken cancellationToken = default);

        Task<bool> IsInAppEnabledAsync(
            string userId,
            NotificationType type,
            CancellationToken cancellationToken = default);
    }
}
