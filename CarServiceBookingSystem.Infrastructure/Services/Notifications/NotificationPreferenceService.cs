using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.NotificationPreferences;
using CarServiceBookingSystem.Application.Interfaces.IContext;
using CarServiceBookingSystem.Application.Interfaces.INotification;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services.Notifications
{
    public class NotificationPreferenceService : INotificationPreferenceService
    {
        private static readonly NotificationType[] MandatoryTypes =
        {
        NotificationType.SecurityAlert
    };

        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public NotificationPreferenceService(
            ApplicationDbContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<List<NotificationPreferenceResponse>>> GetMyPreferencesAsync(
            CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return ApiResponse<List<NotificationPreferenceResponse>>.Fail("User is not authenticated.");
            }

            var existingPreferences = await _context.NotificationPreferences
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);

            var responses = Enum.GetValues<NotificationType>()
                .Select(type =>
                {
                    var existing = existingPreferences.FirstOrDefault(x => x.Type == type);

                    return MapToResponse(type, existing);
                })
                .OrderBy(x => (int)x.Type)
                .ToList();

            return ApiResponse<List<NotificationPreferenceResponse>>.Ok(responses);
        }

        public async Task<ApiResponse<NotificationPreferenceResponse>> UpdateMyPreferenceAsync(
            NotificationType type,
            UpdateNotificationPreferenceRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return ApiResponse<NotificationPreferenceResponse>.Fail("User is not authenticated.");
            }

            if (!Enum.IsDefined(typeof(NotificationType), type))
            {
                return ApiResponse<NotificationPreferenceResponse>.Fail("Invalid notification type.");
            }

            var isMandatory = IsMandatory(type);

            var preference = await _context.NotificationPreferences
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.Type == type,
                    cancellationToken);

            if (preference is null)
            {
                preference = new NotificationPreference
                {
                    UserId = userId,
                    Type = type,
                    CreatedAt = DateTime.UtcNow
                };

                _context.NotificationPreferences.Add(preference);
            }

            preference.InAppEnabled = isMandatory || request.InAppEnabled;
            preference.EmailEnabled = request.EmailEnabled;
            preference.PushEnabled = request.PushEnabled;
            preference.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<NotificationPreferenceResponse>.Ok(
                MapToResponse(type, preference));
        }

        public async Task<ApiResponse<List<NotificationPreferenceResponse>>> BulkUpdateMyPreferencesAsync(
            BulkUpdateNotificationPreferencesRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return ApiResponse<List<NotificationPreferenceResponse>>.Fail("User is not authenticated.");
            }

            if (request.Preferences.Count == 0)
            {
                return ApiResponse<List<NotificationPreferenceResponse>>.Fail("At least one preference is required.");
            }

            var requestedTypes = request.Preferences
                .Select(x => x.Type)
                .Distinct()
                .ToList();

            var existingPreferences = await _context.NotificationPreferences
                .Where(x =>
                    x.UserId == userId &&
                    requestedTypes.Contains(x.Type))
                .ToListAsync(cancellationToken);

            var now = DateTime.UtcNow;

            foreach (var item in request.Preferences)
            {
                if (!Enum.IsDefined(typeof(NotificationType), item.Type))
                {
                    return ApiResponse<List<NotificationPreferenceResponse>>.Fail("Invalid notification type.");
                }

                var preference = existingPreferences.FirstOrDefault(x => x.Type == item.Type);

                if (preference is null)
                {
                    preference = new NotificationPreference
                    {
                        UserId = userId,
                        Type = item.Type,
                        CreatedAt = now
                    };

                    _context.NotificationPreferences.Add(preference);
                    existingPreferences.Add(preference);
                }

                var isMandatory = IsMandatory(item.Type);

                preference.InAppEnabled = isMandatory || item.InAppEnabled;
                preference.EmailEnabled = item.EmailEnabled;
                preference.PushEnabled = item.PushEnabled;
                preference.UpdatedAt = now;
            }

            await _context.SaveChangesAsync(cancellationToken);

            var allPreferences = await _context.NotificationPreferences
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);

            var responses = Enum.GetValues<NotificationType>()
                .Select(type =>
                {
                    var existing = allPreferences.FirstOrDefault(x => x.Type == type);
                    return MapToResponse(type, existing);
                })
                .OrderBy(x => (int)x.Type)
                .ToList();

            return ApiResponse<List<NotificationPreferenceResponse>>.Ok(responses);
        }

        public async Task<bool> IsInAppEnabledAsync(
            string userId,
            NotificationType type,
            CancellationToken cancellationToken = default)
        {
            if (IsMandatory(type))
            {
                return true;
            }

            var preference = await _context.NotificationPreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.Type == type,
                    cancellationToken);

            // Default: enabled unless user explicitly disables it.
            return preference?.InAppEnabled ?? true;
        }

        public async Task<NotificationDeliveryPreferenceResponse> GetDeliveryPreferenceAsync(
    string userId,
    NotificationType type,
    CancellationToken cancellationToken = default)
        {
            var isMandatory = IsMandatory(type);

            var preference = await _context.NotificationPreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.Type == type,
                    cancellationToken);

            return new NotificationDeliveryPreferenceResponse
            {
                Type = type,
                IsMandatory = isMandatory,

                // Mandatory notifications always create in-app DB notification.
                InAppEnabled = isMandatory || preference?.InAppEnabled != false,

                // Push is user-controlled.
                PushEnabled = preference?.PushEnabled == true,

                // Email is user-controlled.
                EmailEnabled = preference?.EmailEnabled == true
            };
        }

        private static NotificationPreferenceResponse MapToResponse(
            NotificationType type,
            NotificationPreference? preference)
        {
            var isMandatory = IsMandatory(type);

            return new NotificationPreferenceResponse
            {
                Type = type,
                TypeName = type.ToString(),
                InAppEnabled = isMandatory || preference?.InAppEnabled != false,
                EmailEnabled = preference?.EmailEnabled == true,
                PushEnabled = preference?.PushEnabled == true,
                IsMandatory = isMandatory,
                UpdatedAt = preference?.UpdatedAt
            };
        }

        private static bool IsMandatory(NotificationType type)
        {
            return MandatoryTypes.Contains(type);
        }
    }
}
