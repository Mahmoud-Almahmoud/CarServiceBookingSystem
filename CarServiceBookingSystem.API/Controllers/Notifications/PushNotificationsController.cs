using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.PushNotifications;
using CarServiceBookingSystem.Application.Interfaces.INotification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers.Notifications;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/push-notifications")]
public class PushNotificationsController : ControllerBase
{
    private readonly IPushNotificationService _pushNotificationService;

    public PushNotificationsController(
        IPushNotificationService pushNotificationService)
    {
        _pushNotificationService = pushNotificationService;
    }

    [AllowAnonymous]
    [HttpGet("public-key")]
    public async Task<IActionResult> GetPublicKey(
        CancellationToken cancellationToken)
    {
        var response = await _pushNotificationService.GetPublicKeyAsync(
            cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [Authorize]
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe(
        [FromBody] PushSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _pushNotificationService.SubscribeAsync(
            request,
            Request.Headers.UserAgent.ToString(),
            cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [Authorize]
    [HttpPost("unsubscribe")]
    public async Task<IActionResult> Unsubscribe(
        [FromBody] PushUnsubscribeRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _pushNotificationService.UnsubscribeAsync(
            request.Endpoint,
            cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }
}

public class PushUnsubscribeRequest
{
    public string Endpoint { get; set; } = string.Empty;
}