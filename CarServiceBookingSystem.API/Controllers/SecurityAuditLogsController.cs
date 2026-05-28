using Asp.Versioning;
using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Auth;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
using CarServiceBookingSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarServiceBookingSystem.API.Controllers;

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
    public async Task<IActionResult> GetMyActivity([FromQuery] PagedRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result = await _queryService.GetMyLogsAsync(userId, request);

        return Ok(result);
    }
}