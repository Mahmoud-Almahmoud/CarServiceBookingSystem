using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Notifications;
using CarServiceBookingSystem.Application.DTOs.PushNotifications;
using CarServiceBookingSystem.Application.Interfaces.IContext;
using CarServiceBookingSystem.Application.Interfaces.INotification;
using CarServiceBookingSystem.Application.Options;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using WebPush;
using DomainPushSubscription = CarServiceBookingSystem.Domain.Entities.PushSubscription;
using WebPushSubscription = WebPush.PushSubscription;

namespace CarServiceBookingSystem.Infrastructure.Services.Notifications;

public class PushNotificationService : IPushNotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly WebPushOptions _options;
    private readonly ILogger<PushNotificationService> _logger;

    public PushNotificationService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        IOptions<WebPushOptions> options,
        ILogger<PushNotificationService> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _options = options.Value;
        _logger = logger;
    }

    public Task<ApiResponse<WebPushPublicKeyResponse>> GetPublicKeyAsync(
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.PublicKey))
        {
            return Task.FromResult(
                ApiResponse<WebPushPublicKeyResponse>.Fail("Web push is not configured."));
        }

        return Task.FromResult(
            ApiResponse<WebPushPublicKeyResponse>.Ok(new WebPushPublicKeyResponse
            {
                PublicKey = _options.PublicKey
            }));
    }

    public async Task<ApiResponse<PushSubscriptionResponse>> SubscribeAsync(
        PushSubscriptionRequest request,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return ApiResponse<PushSubscriptionResponse>.Fail("User is not authenticated.");
        }

        if (string.IsNullOrWhiteSpace(request.Endpoint) ||
            string.IsNullOrWhiteSpace(request.Keys.P256dh) ||
            string.IsNullOrWhiteSpace(request.Keys.Auth))
        {
            return ApiResponse<PushSubscriptionResponse>.Fail("Invalid push subscription.");
        }

        var subscription = await _context.PushSubscriptions
            .FirstOrDefaultAsync(x => x.Endpoint == request.Endpoint, cancellationToken);

        if (subscription is null)
        {
            subscription = new DomainPushSubscription
            {
                UserId = userId,
                Endpoint = request.Endpoint,
                P256dh = request.Keys.P256dh,
                Auth = request.Keys.Auth,
                UserAgent = userAgent,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.PushSubscriptions.Add(subscription);
        }
        else
        {
            subscription.UserId = userId;
            subscription.P256dh = request.Keys.P256dh;
            subscription.Auth = request.Keys.Auth;
            subscription.UserAgent = userAgent;
            subscription.IsActive = true;
            subscription.RevokedAt = null;
            subscription.LastUsedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<PushSubscriptionResponse>.Ok(MapToResponse(subscription));
    }

    public async Task<ApiResponse<bool>> UnsubscribeAsync(
        string endpoint,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return ApiResponse<bool>.Fail("User is not authenticated.");
        }

        var subscription = await _context.PushSubscriptions
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.Endpoint == endpoint,
                cancellationToken);

        if (subscription is null)
        {
            return ApiResponse<bool>.Ok(true);
        }

        subscription.IsActive = false;
        subscription.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }

    public async Task SendToUserAsync(
        string userId,
        NotificationResponse notification,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
            return;

        if (string.IsNullOrWhiteSpace(_options.Subject) ||
            string.IsNullOrWhiteSpace(_options.PublicKey) ||
            string.IsNullOrWhiteSpace(_options.PrivateKey))
        {
            _logger.LogWarning("Web push is enabled but VAPID settings are missing.");
            return;
        }

        var subscriptions = await _context.PushSubscriptions
            .Where(x =>
                x.UserId == userId &&
                x.IsActive)
            .ToListAsync(cancellationToken);

        if (subscriptions.Count == 0)
            return;

        var vapidDetails = new VapidDetails(
            _options.Subject,
            _options.PublicKey,
            _options.PrivateKey);

        var client = new WebPushClient();

        var payload = JsonSerializer.Serialize(new
        {
            title = notification.Title,
            body = notification.Message,
            icon = "/icons/icon-192.png",
            badge = "/icons/badge-72.png",
            url = notification.ActionUrl ?? "/notifications",
            notificationId = notification.Id,
            type = notification.Type.ToString(),
            severity = notification.Severity.ToString(),
            createdAt = notification.CreatedAt
        });

        foreach (var savedSubscription in subscriptions)
        {
            try
            {
                var webPushSubscription = new WebPushSubscription(
                    savedSubscription.Endpoint,
                    savedSubscription.P256dh,
                    savedSubscription.Auth);

                await client.SendNotificationAsync(
                    webPushSubscription,
                    payload,
                    vapidDetails,
                    cancellationToken);

                savedSubscription.LastUsedAt = DateTime.UtcNow;
            }
            catch (WebPushException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Web push failed for subscription {SubscriptionId}. StatusCode {StatusCode}",
                    savedSubscription.Id,
                    ex.StatusCode);

                if (ex.StatusCode == HttpStatusCode.Gone ||
                    ex.StatusCode == HttpStatusCode.NotFound)
                {
                    savedSubscription.IsActive = false;
                    savedSubscription.RevokedAt = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected web push error for subscription {SubscriptionId}",
                    savedSubscription.Id);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static PushSubscriptionResponse MapToResponse(
        DomainPushSubscription subscription)
    {
        return new PushSubscriptionResponse
        {
            Id = subscription.Id,
            Endpoint = subscription.Endpoint,
            IsActive = subscription.IsActive,
            CreatedAt = subscription.CreatedAt
        };
    }
}