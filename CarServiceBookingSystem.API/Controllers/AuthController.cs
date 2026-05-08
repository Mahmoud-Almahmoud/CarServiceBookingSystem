using Asp.Versioning;
using CarServiceBookingSystem.API.Filters;
using CarServiceBookingSystem.Application.DTOs.Auth;
using CarServiceBookingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ServiceFilter(typeof(ValidationFilter<RegisterRequest>))]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("login")]
    [ServiceFilter(typeof(ValidationFilter<LoginRequest>))]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request)
    {
        var result = await _authService.LogoutAsync(request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}