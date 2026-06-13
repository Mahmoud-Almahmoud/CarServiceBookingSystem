using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.NotificationPreferences;
using CarServiceBookingSystem.Application.Interfaces.INotification;
using CarServiceBookingSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers.Notifications;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notification-preferences")]
public class NotificationPreferencesController : ControllerBase
{
    private readonly INotificationPreferenceService _notificationPreferenceService;

    public NotificationPreferencesController(
        INotificationPreferenceService notificationPreferenceService)
    {
        _notificationPreferenceService = notificationPreferenceService;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyPreferences(
        CancellationToken cancellationToken)
    {
        var response = await _notificationPreferenceService.GetMyPreferencesAsync(
            cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPut("my/{type}")]
    public async Task<IActionResult> UpdateMyPreference(
        NotificationType type,
        [FromBody] UpdateNotificationPreferenceRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _notificationPreferenceService.UpdateMyPreferenceAsync(
            type,
            request,
            cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPut("my")]
    public async Task<IActionResult> BulkUpdateMyPreferences(
        [FromBody] BulkUpdateNotificationPreferencesRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _notificationPreferenceService.BulkUpdateMyPreferencesAsync(
            request,
            cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }
}