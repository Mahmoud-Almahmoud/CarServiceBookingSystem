using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.NotificationBroadcasts;
using CarServiceBookingSystem.Application.Interfaces.INotification;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers.Notifications;

[ApiController]
[Authorize(Policy = Permissions.Notifications.Broadcast)]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/notification-broadcasts")]
public class AdminNotificationBroadcastsController : ControllerBase
{
    private readonly INotificationBroadcastService _notificationBroadcastService;

    public AdminNotificationBroadcastsController(
        INotificationBroadcastService notificationBroadcastService)
    {
        _notificationBroadcastService = notificationBroadcastService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send(
        [FromBody] BroadcastNotificationRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _notificationBroadcastService.SendAsync(
            request,
            cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }
}