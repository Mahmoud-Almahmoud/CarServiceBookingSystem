using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.Notifications;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Interfaces.INotification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers.Notifications;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyNotifications(
        [FromQuery] NotificationQueryRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _notificationService.GetMyNotificationsAsync(
            request,
            cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetMyUnreadCount(
        CancellationToken cancellationToken)
    {
        var response = await _notificationService.GetMyUnreadCountAsync(
            cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _notificationService.MarkAsReadAsync(
            id,
            cancellationToken);

        if (!response.Success)
        {
            if (response.Message == "Notification not found.")
                return NotFound(response);

            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead(
        CancellationToken cancellationToken)
    {
        var response = await _notificationService.MarkAllAsReadAsync(
            cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _notificationService.DeleteAsync(
            id,
            cancellationToken);

        if (!response.Success)
        {
            if (response.Message == "Notification not found.")
                return NotFound(response);

            return BadRequest(response);
        }

        return Ok(response);
    }
}