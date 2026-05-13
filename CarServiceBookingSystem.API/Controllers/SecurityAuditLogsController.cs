using Asp.Versioning;
using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/security-audit-logs")]
[Authorize(Roles = Roles.Admin)]
public class SecurityAuditLogsController : ControllerBase
{
    private readonly ISecurityAuditQueryService _queryService;

    public SecurityAuditLogsController(ISecurityAuditQueryService queryService)
    {
        _queryService = queryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLogs([FromQuery] PagedRequest request)
    {
        var result = await _queryService.GetLogsAsync(request);
        return Ok(result);
    }
}