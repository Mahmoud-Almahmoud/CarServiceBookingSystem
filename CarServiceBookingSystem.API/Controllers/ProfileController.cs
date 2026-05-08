using Asp.Versioning;
using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;

    public ProfileController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        return Ok(ApiResponse<object>.Ok(new
        {
            userId = _currentUserService.UserId,
            email = _currentUserService.Email,
            isAuthenticated = _currentUserService.IsAuthenticated
        }));
    }
}