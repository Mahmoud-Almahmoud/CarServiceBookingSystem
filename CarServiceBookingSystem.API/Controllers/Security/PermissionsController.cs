using Asp.Versioning;
using CarServiceBookingSystem.Application.Interfaces.ISecurity;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/permissions")]
public sealed class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    [Authorize(Policy = Permissions.PermissionsCatalog.View)]
    public IActionResult GetAll([FromQuery] string? group, [FromQuery] string? search)
    {
        var response = _permissionService.GetAll(group, search);
        return Ok(response);
    }
}