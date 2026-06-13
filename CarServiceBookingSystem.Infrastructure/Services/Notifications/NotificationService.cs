using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Notifications;
using CarServiceBookingSystem.Application.Interfaces.IContext;
using CarServiceBookingSystem.Application.Interfaces.INotification;
using CarServiceBookingSystem.Infrastructure.Persistence;
using FluentValidation;
using CarServiceBookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services.Notifications;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<CreateNotificationRequest> _createValidator;
    private readonly INotificationRealtimeService _notificationRealtimeService;
    private readonly INotificationPreferenceService _notificationPreferenceService;

    public NotificationService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        IValidator<CreateNotificationRequest> createValidator,
        INotificationRealtimeService notificationRealtimeService,
        INotificationPreferenceService notificationPreferenceService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _createValidator = createValidator;
        _notificationRealtimeService = notificationRealtimeService;
        _notificationPreferenceService = notificationPreferenceService;
    }

    public async Task<ApiResponse<NotificationResponse>> CreateAsync(
        CreateNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<NotificationResponse>.Fail(errors);
        }

        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.UserId, cancellationToken);

        if (!userExists)
        {
            return ApiResponse<NotificationResponse>.Fail("User not found.");
        }

        var inAppEnabled = await _notificationPreferenceService.IsInAppEnabledAsync(
            request.UserId,
            request.Type,
            cancellationToken);

        if (!inAppEnabled)
        {
            return ApiResponse<NotificationResponse>.Ok(new NotificationResponse
            {
                Id = 0,
                Title = request.Title,
                Message = request.Message,
                Type = request.Type,
                Severity = request.Severity,
                EntityType = request.EntityType,
                EntityId = request.EntityId,
                ActionUrl = request.ActionUrl,
                IsRead = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        var notification = new Notification
        {
            UserId = request.UserId,
            Title = request.Title.Trim(),
            Message = request.Message.Trim(),
            Type = request.Type,
            Severity = request.Severity,
            EntityType = string.IsNullOrWhiteSpace(request.EntityType)
                ? null
                : request.EntityType.Trim(),
            EntityId = request.EntityId,
            ActionUrl = string.IsNullOrWhiteSpace(request.ActionUrl)
                ? null
                : request.ActionUrl.Trim(),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync(cancellationToken);

        var response = MapToResponse(notification);

        var unreadCount = await _context.Notifications
            .AsNoTracking()
            .CountAsync(x =>
                x.UserId == notification.UserId &&
                !x.IsRead,
                cancellationToken);

        await _notificationRealtimeService.SendNotificationAsync(
            notification.UserId,
            response,
            cancellationToken);

        await _notificationRealtimeService.SendUnreadCountAsync(
            notification.UserId,
            unreadCount,
            cancellationToken);

        return ApiResponse<NotificationResponse>.Ok(response);
    }

    public async Task<ApiResponse<List<NotificationResponse>>> CreateManyAsync(
        List<CreateNotificationRequest> requests,
        CancellationToken cancellationToken = default)
    {
        if (requests.Count == 0)
        {
            return ApiResponse<List<NotificationResponse>>.Fail("At least one notification is required.");
        }

        var notifications = new List<Notification>();
        var skippedResponses = new List<NotificationResponse>();

        foreach (var request in requests)
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage));
                return ApiResponse<List<NotificationResponse>>.Fail(errors);
            }

            var inAppEnabled = await _notificationPreferenceService.IsInAppEnabledAsync(
                request.UserId,
                request.Type,
                cancellationToken);

            if (!inAppEnabled)
            {
                skippedResponses.Add(new NotificationResponse
                {
                    Id = 0,
                    Title = request.Title,
                    Message = request.Message,
                    Type = request.Type,
                    Severity = request.Severity,
                    EntityType = request.EntityType,
                    EntityId = request.EntityId,
                    ActionUrl = request.ActionUrl,
                    IsRead = true,
                    CreatedAt = DateTime.UtcNow
                });

                continue;
            }

            notifications.Add(new Notification
            {
                UserId = request.UserId,
                Title = request.Title.Trim(),
                Message = request.Message.Trim(),
                Type = request.Type,
                Severity = request.Severity,
                EntityType = string.IsNullOrWhiteSpace(request.EntityType)
                    ? null
                    : request.EntityType.Trim(),
                EntityId = request.EntityId,
                ActionUrl = string.IsNullOrWhiteSpace(request.ActionUrl)
                    ? null
                    : request.ActionUrl.Trim(),
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (notifications.Count == 0)
        {
            return ApiResponse<List<NotificationResponse>>.Ok(skippedResponses);
        }

        var userIds = notifications
            .Select(x => x.UserId)
            .Distinct()
            .ToList();

        var existingUserIds = await _context.Users
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var missingUserId = userIds.FirstOrDefault(x => !existingUserIds.Contains(x));

        if (!string.IsNullOrWhiteSpace(missingUserId))
        {
            return ApiResponse<List<NotificationResponse>>.Fail($"User not found: {missingUserId}");
        }

        _context.Notifications.AddRange(notifications);

        await _context.SaveChangesAsync(cancellationToken);

        var responseByUser = notifications
                .Select(x => new
                {
                    x.UserId,
                    Response = MapToResponse(x)
                })
                .ToList();

        foreach (var userGroup in responseByUser.GroupBy(x => x.UserId))
        {
            var unreadCount = await _context.Notifications
                .AsNoTracking()
                .CountAsync(x =>
                    x.UserId == userGroup.Key &&
                    !x.IsRead,
                    cancellationToken);

            foreach (var item in userGroup)
            {
                await _notificationRealtimeService.SendNotificationAsync(
                    userGroup.Key,
                    item.Response,
                    cancellationToken);
            }

            await _notificationRealtimeService.SendUnreadCountAsync(
                userGroup.Key,
                unreadCount,
                cancellationToken);
        }

        var finalResponse = responseByUser
            .Select(x => x.Response)
            .Concat(skippedResponses)
            .ToList();

        return ApiResponse<List<NotificationResponse>>.Ok(finalResponse);
    }

    public async Task<ApiResponse<PagedResponse<NotificationResponse>>> GetMyNotificationsAsync(
        NotificationQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return ApiResponse<PagedResponse<NotificationResponse>>.Fail("User is not authenticated.");
        }

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _context.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        if (request.IsRead.HasValue)
        {
            query = query.Where(x => x.IsRead == request.IsRead.Value);
        }

        if (request.Type.HasValue)
        {
            query = query.Where(x => x.Type == request.Type.Value);
        }

        if (request.Severity.HasValue)
        {
            query = query.Where(x => x.Severity == request.Severity.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.EntityType))
        {
            var entityType = request.EntityType.Trim();
            query = query.Where(x => x.EntityType == entityType);
        }

        if (request.EntityId.HasValue)
        {
            query = query.Where(x => x.EntityId == request.EntityId.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= request.ToDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.Title.Contains(search) ||
                x.Message.Contains(search));
        }

        query = request.SortBy?.ToLower() switch
        {
            "type" => request.Desc
                ? query.OrderByDescending(x => x.Type).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Type).ThenBy(x => x.Id),

            "severity" => request.Desc
                ? query.OrderByDescending(x => x.Severity).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Severity).ThenBy(x => x.Id),

            "isread" => request.Desc
                ? query.OrderByDescending(x => x.IsRead).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.IsRead).ThenBy(x => x.Id),

            "createdat" => request.Desc
                ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new NotificationResponse
            {
                Id = x.Id,
                Title = x.Title,
                Message = x.Message,
                Type = x.Type,
                Severity = x.Severity,
                EntityType = x.EntityType,
                EntityId = x.EntityId,
                ActionUrl = x.ActionUrl,
                IsRead = x.IsRead,
                ReadAt = x.ReadAt,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResponse<NotificationResponse>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return ApiResponse<PagedResponse<NotificationResponse>>.Ok(pagedResponse);
    }

    public async Task<ApiResponse<UnreadNotificationCountResponse>> GetMyUnreadCountAsync(
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return ApiResponse<UnreadNotificationCountResponse>.Fail("User is not authenticated.");
        }

        var count = await _context.Notifications
            .AsNoTracking()
            .CountAsync(x =>
                x.UserId == userId &&
                !x.IsRead,
                cancellationToken);

        return ApiResponse<UnreadNotificationCountResponse>.Ok(new UnreadNotificationCountResponse
        {
            Count = count
        });
    }

    public async Task<ApiResponse<NotificationReadResponse>> MarkAsReadAsync(
        int notificationId,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return ApiResponse<NotificationReadResponse>.Fail("User is not authenticated.");
        }

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(x =>
                x.Id == notificationId &&
                x.UserId == userId,
                cancellationToken);

        if (notification is null)
        {
            return ApiResponse<NotificationReadResponse>.Fail("Notification not found.");
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        var response = new NotificationReadResponse
        {
            NotificationId = notification.Id,
            IsRead = notification.IsRead,
            ReadAt = notification.ReadAt
        };

        var unreadCount = await _context.Notifications
            .AsNoTracking()
            .CountAsync(x =>
                x.UserId == userId &&
                !x.IsRead,
                cancellationToken);

        await _notificationRealtimeService.SendNotificationReadAsync(
            userId,
            response,
            cancellationToken);

        await _notificationRealtimeService.SendUnreadCountAsync(
            userId,
            unreadCount,
            cancellationToken);

        return ApiResponse<NotificationReadResponse>.Ok(response);
    }

    public async Task<ApiResponse<NotificationsReadAllResponse>> MarkAllAsReadAsync(
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return ApiResponse<NotificationsReadAllResponse>.Fail("User is not authenticated.");
        }

        var unreadNotifications = await _context.Notifications
            .Where(x =>
                x.UserId == userId &&
                !x.IsRead)
            .ToListAsync(cancellationToken);

        if (unreadNotifications.Count == 0)
        {
            return ApiResponse<NotificationsReadAllResponse>.Ok(new NotificationsReadAllResponse
            {
                UpdatedCount = 0
            });
        }

        var now = DateTime.UtcNow;

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = now;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var response = new NotificationsReadAllResponse
        {
            UpdatedCount = unreadNotifications.Count
        };

        await _notificationRealtimeService.SendNotificationsReadAllAsync(
            userId,
            response,
            cancellationToken);

        await _notificationRealtimeService.SendUnreadCountAsync(
            userId,
            0,
            cancellationToken);

        return ApiResponse<NotificationsReadAllResponse>.Ok(response);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(
        int notificationId,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return ApiResponse<bool>.Fail("User is not authenticated.");
        }

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(x =>
                x.Id == notificationId &&
                x.UserId == userId,
                cancellationToken);

        if (notification is null)
        {
            return ApiResponse<bool>.Fail("Notification not found.");
        }

        _context.Notifications.Remove(notification);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }

    private static NotificationResponse MapToResponse(Notification notification)
    {
        return new NotificationResponse
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type,
            Severity = notification.Severity,
            EntityType = notification.EntityType,
            EntityId = notification.EntityId,
            ActionUrl = notification.ActionUrl,
            IsRead = notification.IsRead,
            ReadAt = notification.ReadAt,
            CreatedAt = notification.CreatedAt
        };
    }
}