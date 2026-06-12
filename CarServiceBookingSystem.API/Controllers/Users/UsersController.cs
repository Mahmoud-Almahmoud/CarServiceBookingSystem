using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.Users;
using CarServiceBookingSystem.Application.Interfaces.IUsers;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers.Users;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/admin/users")]
[Authorize(Policy = Permissions.Users.View)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] UserRequest query)
    {
        var result = await _userService.GetUsersAsync(query);
        return Ok(result);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var result = await _userService.GetUserByIdAsync(userId);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost("{userId}/lock")]
    [Authorize(Policy = Permissions.Users.Manage)]
    public async Task<IActionResult> LockUser(string userId)
    {
        var result = await _userService.LockUserAsync(userId);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost("{userId}/unlock")]
    [Authorize(Policy = Permissions.Users.Manage)]
    public async Task<IActionResult> UnlockUser(string userId)
    {
        var result = await _userService.UnlockUserAsync(userId);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost("{userId}/revoke-sessions")]
    [Authorize(Policy = Permissions.Users.Manage)]
    public async Task<IActionResult> RevokeUserSessions(string userId)
    {
        var result = await _userService.RevokeUserSessionsAsync(userId);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet("{userId}/roles")]
    [Authorize(Policy = Permissions.Users.View)]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var result = await _userService.GetUserRolesAsync(userId);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost("{userId}/roles")]
    [Authorize(Policy = Permissions.Users.Manage)]
    public async Task<IActionResult> AddUserRole(
    string userId,
    [FromBody] UpdateUserRoleRequest request)
    {
        var result = await _userService.AddUserRoleAsync(userId, request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{userId}/roles")]
    [Authorize(Policy = Permissions.Users.Manage)]
    public async Task<IActionResult> RemoveUserRole(
    string userId,
    [FromBody] UpdateUserRoleRequest request)
    {
        var result = await _userService.RemoveUserRoleAsync(userId, request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}