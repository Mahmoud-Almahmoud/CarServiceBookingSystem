using Asp.Versioning;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

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

    public MaintenanceController(
        IRefreshTokenCleanupService refreshTokenCleanupService,
        ITrustedDeviceCleanupService trustedDeviceCleanupService,
        ISecurityAuditLogCleanupService securityAuditLogCleanupService,
        IStripeWebhookCleanupService stripeWebhookCleanupService,
        IIdempotencyCleanupService idempotencyCleanupService)
    {
        _refreshTokenCleanupService = refreshTokenCleanupService;
        _trustedDeviceCleanupService = trustedDeviceCleanupService;
        _securityAuditLogCleanupService = securityAuditLogCleanupService;
        _stripeWebhookCleanupService = stripeWebhookCleanupService;
        _idempotencyCleanupService = idempotencyCleanupService;
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

    [HttpPost("cleanup/all")]
    public async Task<IActionResult> CleanupAll()
    {
        await _refreshTokenCleanupService.CleanupExpiredAndOldRevokedTokensAsync();
        await _trustedDeviceCleanupService.CleanupExpiredTrustedDevicesAsync();
        await _securityAuditLogCleanupService.CleanupOldLogsAsync();
        await _stripeWebhookCleanupService.CleanupOldWebhookEventsAsync();
        await _idempotencyCleanupService.CleanupExpiredKeysAsync();

        return Ok(new { message = "All cleanup jobs completed." });
    }
}