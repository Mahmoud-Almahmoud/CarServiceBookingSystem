using Asp.Versioning;
using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs;
using CarServiceBookingSystem.Application.Security;
using CarServiceBookingSystem.Infrastructure.Services.BackgroundJobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers.Admin;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/admin/maintenance")]
[Authorize(Policy = Permissions.Maintenance.Manage)]
public class MaintenanceController : ControllerBase
{
    private readonly IRefreshTokenCleanupService _refreshTokenCleanupService;
    private readonly ITrustedDeviceCleanupService _trustedDeviceCleanupService;
    private readonly ISecurityAuditLogCleanupService _securityAuditLogCleanupService;
    private readonly IStripeWebhookCleanupService _stripeWebhookCleanupService;
    private readonly IIdempotencyCleanupService _idempotencyCleanupService;
    private readonly IBookingCleanupJob _bookingCleanupJob;

    public MaintenanceController(
        IRefreshTokenCleanupService refreshTokenCleanupService,
        ITrustedDeviceCleanupService trustedDeviceCleanupService,
        ISecurityAuditLogCleanupService securityAuditLogCleanupService,
        IStripeWebhookCleanupService stripeWebhookCleanupService,
        IIdempotencyCleanupService idempotencyCleanupService,
        IBookingCleanupJob bookingCleanupJob)
    {
        _refreshTokenCleanupService = refreshTokenCleanupService;
        _trustedDeviceCleanupService = trustedDeviceCleanupService;
        _securityAuditLogCleanupService = securityAuditLogCleanupService;
        _stripeWebhookCleanupService = stripeWebhookCleanupService;
        _idempotencyCleanupService = idempotencyCleanupService;
        _bookingCleanupJob = bookingCleanupJob;
    }

    [HttpPost("cleanup/refresh-tokens")]
    public async Task<IActionResult> CleanupRefreshTokens()
    {
        await _refreshTokenCleanupService.CleanupExpiredAndOldRevokedTokensAsync();
        return Ok(new { message = "Refresh token cleanup completed." });
    }

    [HttpPost("cleanup/trusted-devices")]
    public async Task<IActionResult> CleanupTrustedDevices()
    {
        await _trustedDeviceCleanupService.CleanupExpiredTrustedDevicesAsync();
        return Ok(new { message = "Trusted device cleanup completed." });
    }

    [HttpPost("cleanup/security-logs")]
    public async Task<IActionResult> CleanupSecurityLogs()
    {
        await _securityAuditLogCleanupService.CleanupOldLogsAsync();
        return Ok(new { message = "Security audit log cleanup completed." });
    }

    [HttpPost("cleanup/webhook-events")]
    public async Task<IActionResult> CleanupWebhookEvents()
    {
        await _stripeWebhookCleanupService.CleanupOldWebhookEventsAsync();
        return Ok(new { message = "Stripe webhook event cleanup completed." });
    }

    [HttpPost("cleanup/idempotency-keys")]
    public async Task<IActionResult> CleanupIdempotencyKeys()
    {
        await _idempotencyCleanupService.CleanupExpiredKeysAsync();
        return Ok(new { message = "Idempotency key cleanup completed." });
    }

    [HttpPost("cleanup/pending-bookings")]
    [Authorize(Policy = Permissions.Maintenance.Manage)]
    public async Task<IActionResult> CancelStalePendingBookings(CancellationToken cancellationToken)
    {
        await _bookingCleanupJob.CancelStalePendingBookingsAsync(cancellationToken);
        return Ok(ApiResponse<string>.Ok("Booking cleanup completed."));
    }

    [HttpPost("cleanup/notifications")]
    [Authorize(Policy = Permissions.Maintenance.Manage)]
    public async Task<IActionResult> CleanupNotifications(
    [FromServices] INotificationCleanupJob notificationCleanupJob,
    CancellationToken cancellationToken)
    {
        var count = await notificationCleanupJob.DeleteOldNotificationsAsync(cancellationToken);
        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Notification cleanup completed.",
            Data = $"{count} notifications has been cleaned up."
        });
    }

    [HttpPost("cleanup/all")]
    public async Task<IActionResult> CleanupAll(CancellationToken cancellationToken)
    {
        await _refreshTokenCleanupService.CleanupExpiredAndOldRevokedTokensAsync();
        await _trustedDeviceCleanupService.CleanupExpiredTrustedDevicesAsync();
        await _securityAuditLogCleanupService.CleanupOldLogsAsync();
        await _stripeWebhookCleanupService.CleanupOldWebhookEventsAsync();
        await _idempotencyCleanupService.CleanupExpiredKeysAsync();
        await _bookingCleanupJob.CancelStalePendingBookingsAsync(cancellationToken);

        return Ok(new { message = "All cleanup jobs completed." });
    }
}