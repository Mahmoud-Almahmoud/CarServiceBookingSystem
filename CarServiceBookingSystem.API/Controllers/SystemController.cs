using Asp.Versioning;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/admin/system")]
[Authorize(Policy = Permissions.System.View)]
public class SystemController : ControllerBase
{
    private readonly ISystemStatusService _systemStatusService;

    public SystemController(ISystemStatusService systemStatusService)
    {
        _systemStatusService = systemStatusService;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var status = await _systemStatusService.GetStatusAsync();
        return Ok(status);
    }

    [HttpGet("configuration")]
    public IActionResult GetConfiguration()
    {
        var configuration = _systemStatusService.GetConfiguration();
        return Ok(configuration);
    }
}