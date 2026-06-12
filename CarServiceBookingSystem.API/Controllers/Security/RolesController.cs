using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.Roles;
using CarServiceBookingSystem.Application.Interfaces.ISecurity;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/admin/roles")]
[Authorize(Policy = Permissions.Users.Manage)]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Roles.Create)]
    public async Task<IActionResult> Create(CreateRoleRequest request)
    {
        var result = await _roleService.CreateAsync(request);

        return result.Success? Ok(result): BadRequest(result);
    }

    [HttpPut("{roleId}")]
    [Authorize(Policy = Permissions.Roles.Update)]
    public async Task<IActionResult> Update(
        string roleId,
        UpdateRoleRequest request)
    {
        var result = await _roleService.UpdateAsync(roleId, request);

        return result.Success? Ok(result): BadRequest(result);
    }

    [HttpDelete("{roleId}")]
    [Authorize(Policy = Permissions.Roles.Delete)]
    public async Task<IActionResult> Delete(string roleId)
    {
        var result = await _roleService.DeleteAsync(roleId);

        return result.Success? Ok(result): BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var result = await _roleService.GetRolesAsync();

        return result.Success? Ok(result): BadRequest(result);
    }

    [HttpGet("{roleId}")]
    public async Task<IActionResult> GetRoleById(string roleId)
    {
        var result = await _roleService.GetRoleByIdAsync(roleId);

        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("{roleId}/permissions")]
    [Authorize(Policy = Permissions.Users.Manage)]
    public async Task<IActionResult> GetRolePermissions(string roleId)
    {
        var result = await _roleService.GetRolePermissionsAsync(roleId);

        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("{roleId}/permissions")]
    [Authorize(Policy = Permissions.Users.Manage)]
    public async Task<IActionResult> AddRolePermission(
    string roleId,
    [FromBody] UpdateRolePermissionRequest request)
    {
        var result = await _roleService.AddRolePermissionAsync(roleId, request);

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{roleId}/permissions")]
    [Authorize(Policy = Permissions.Users.Manage)]
    public async Task<IActionResult> RemoveRolePermission(
    string roleId,
    [FromBody] UpdateRolePermissionRequest request)
    {
        var result = await _roleService.RemoveRolePermissionAsync(roleId, request);

        return result.Success ? Ok(result) : BadRequest(result);
    }
}