using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.Notifications;
using CarServiceBookingSystem.Application.Interfaces.INotification;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers.Admin;

[ApiController]
[Authorize(Policy = Permissions.Notifications.Send)]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/notifications")]
public class AdminNotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public AdminNotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send(
        [FromBody] AdminSendNotificationRequest request,
        CancellationToken cancellationToken)
    {
        var createRequest = new CreateNotificationRequest
        {
            UserId = request.UserId,
            Title = request.Title,
            Message = request.Message,
            Type = request.Type,
            Severity = request.Severity,
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            ActionUrl = request.ActionUrl
        };

        var response = await _notificationService.CreateAsync(
            createRequest,
            cancellationToken);

        if (!response.Success)
        {
            if (response.Message == "User not found.")
                return NotFound(response);

            return BadRequest(response);
        }

        return Ok(response);
    }
}