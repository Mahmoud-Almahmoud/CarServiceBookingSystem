using Asp.Versioning;
using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Auth;
using CarServiceBookingSystem.Application.DTOs.Security;
using CarServiceBookingSystem.Application.Interfaces.ISecurity;
using CarServiceBookingSystem.Application.Security;
using CarServiceBookingSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarServiceBookingSystem.API.Controllers.Security;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/security-audit-logs")]
[Authorize]
public class SecurityAuditLogsController : ControllerBase
{
    private readonly ISecurityAuditQueryService _queryService;

    public SecurityAuditLogsController(ISecurityAuditQueryService queryService)
    {
        _queryService = queryService;
    }

    [HttpGet]
    [Authorize(Policy = Permissions.SecurityAudit.ViewAll)]
    public async Task<IActionResult> GetLogs([FromQuery] SecurityAuditLogRequest request)
    {
        var result = await _queryService.GetLogsAsync(request);
        return Ok(result);
    }

    [HttpGet("my-activity")]
    [Authorize(Policy = Permissions.SecurityAudit.ViewMine)]
    public async Task<IActionResult> GetMyActivity([FromQuery] MyActivityRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result = await _queryService.GetMyLogsAsync(userId, request);

        return Ok(result);
    }

    [HttpGet("event-summary")]
    [Authorize(Policy = Permissions.SecurityAudit.ViewAll)]
    public async Task<IActionResult> GetSummary([FromQuery] int days = 7)
    {
        var response = await _queryService.GetSummaryAsync(days);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("top-countries")]
    [Authorize(Policy = Permissions.SecurityAudit.ViewAll)]
    public async Task<IActionResult> GetTopCountries(
    [FromQuery] int days = 7,
    [FromQuery] int take = 10)
    {
        var response = await _queryService.GetTopCountriesAsync(days, take);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("top-users")]
    [Authorize(Policy = Permissions.SecurityAudit.ViewAll)]
    public async Task<IActionResult> GetTopUsers(
    [FromQuery] int days = 7,
    [FromQuery] int take = 10)
    {
        var response = await _queryService.GetTopUsersAsync(days, take);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("failed-logins")]
    [Authorize(Policy = Permissions.SecurityAudit.ViewAll)]
    public async Task<IActionResult> GetFailedLogins(
    [FromQuery] int days = 7,
    [FromQuery] int take = 10)
    {
        var response = await _queryService.GetFailedLoginsAsync(days, take);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("suspicious-logins")]
    [Authorize(Policy = Permissions.SecurityAudit.ViewAll)]
    public async Task<IActionResult> GetSuspiciousLogins(
    [FromQuery] int days = 7,
    [FromQuery] int take = 10)
    {
        var response = await _queryService.GetSuspiciousLoginsAsync(days, take);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("risky-ips")]
    [Authorize(Policy = Permissions.SecurityAudit.ViewAll)]
    public async Task<IActionResult> GetRiskyIps(
    [FromQuery] int days = 7,
    [FromQuery] int take = 10)
    {
        var response = await _queryService.GetRiskyIpsAsync(days, take);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("access-denied")]
    [Authorize(Policy = Permissions.SecurityAudit.ViewAll)]
    public async Task<IActionResult> GetAccessDenied(
    [FromQuery] int days = 7,
    [FromQuery] int take = 10)
    {
        var response = await _queryService.GetAccessDeniedAsync(days, take);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("daily-trend")]
    [Authorize(Policy = Permissions.SecurityAudit.ViewAll)]
    public async Task<IActionResult> GetDailyTrend([FromQuery] int days = 30)
    {
        var response = await _queryService.GetDailyTrendAsync(days);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("hourly-risk-trend")]
    [Authorize(Policy = Permissions.SecurityAudit.ViewAll)]
    public async Task<IActionResult> GetHourlyRiskTrend([FromQuery] int hours = 24)
    {
        var response = await _queryService.GetHourlyRiskTrendAsync(hours);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("security-score")]
    [Authorize(Policy = Permissions.SecurityAudit.ViewAll)]
    public async Task<IActionResult> GetSecurityScore()
    {
        var response = await _queryService.GetSecurityScoreAsync();
        return Ok(response);
    }
}