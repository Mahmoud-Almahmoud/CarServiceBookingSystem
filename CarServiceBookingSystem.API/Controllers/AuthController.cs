using Asp.Versioning;
using CarServiceBookingSystem.API.Filters;
using CarServiceBookingSystem.Application.DTOs.Auth;
using CarServiceBookingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

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

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId,[FromQuery] string token)
    {
        var result = await _authService.ConfirmEmailAsync(userId, token);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPost("resend-email-confirmation")]
    [ServiceFilter(typeof(ValidationFilter<ResendEmailConfirmationRequest>))]
    public async Task<IActionResult> ResendEmailConfirmation(
    ResendEmailConfirmationRequest request)
    {
        var result = await _authService.ResendEmailConfirmationAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("forgot-password")]
    [ServiceFilter(typeof(ValidationFilter<ForgotPasswordRequest>))]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var result = await _authService.ForgotPasswordAsync(request);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    [ServiceFilter(typeof(ValidationFilter<ResetPasswordRequest>))]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("change-password")]
    [ServiceFilter(typeof(ValidationFilter<ChangePasswordRequest>))]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var result = await _authService.ChangePasswordAsync(request);

        if (!result.Success)
            return BadRequest(result);

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

    [Authorize]
    [HttpPost("logout-all-devices")]
    public async Task<IActionResult> LogoutAllDevices()
    {
        var result = await _authService.LogoutAllDevicesAsync();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("sessions")]
    public async Task<IActionResult> GetActiveSessions()
    {
        var result = await _authService.GetActiveSessionsAsync();
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("sessions/{sessionId:int}")]
    public async Task<IActionResult> RevokeSession(int sessionId)
    {
        var result = await _authService.RevokeSessionAsync(sessionId);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("2fa/setup")]
    public async Task<IActionResult> GetTwoFactorSetup()
    {
        var result = await _authService.GetTwoFactorSetupAsync();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [EnableRateLimiting("TwoFactorPolicy")]
    [HttpPost("2fa/enable")]
    [ServiceFilter(typeof(ValidationFilter<VerifyTwoFactorRequest>))]
    public async Task<IActionResult> EnableTwoFactor(VerifyTwoFactorRequest request)
    {
        var result = await _authService.EnableTwoFactorAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("2fa/login")]
    [EnableRateLimiting("TwoFactorPolicy")]
    [ServiceFilter(typeof(ValidationFilter<LoginTwoFactorRequest>))]
    public async Task<IActionResult> LoginWithTwoFactor(LoginTwoFactorRequest request)
    {
        var result = await _authService.LoginWithTwoFactorAsync(request);

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("2fa/disable")]
    [EnableRateLimiting("TwoFactorPolicy")]
    [ServiceFilter(typeof(ValidationFilter<DisableTwoFactorRequest>))]
    public async Task<IActionResult> DisableTwoFactor(DisableTwoFactorRequest request)
    {
        var result = await _authService.DisableTwoFactorAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("2fa/recovery-codes")]
    public async Task<IActionResult> GenerateRecoveryCodes()
    {
        var result = await _authService.GenerateRecoveryCodesAsync();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("2fa/recovery-login")]
    [EnableRateLimiting("TwoFactorPolicy")]
    [ServiceFilter(typeof(ValidationFilter<LoginRecoveryCodeRequest>))]
    public async Task<IActionResult> LoginWithRecoveryCode(
    LoginRecoveryCodeRequest request)
    {
        var result =
            await _authService.LoginWithRecoveryCodeAsync(request);

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("2fa/qr-code")]
    public async Task<IActionResult> GetTwoFactorQrCode()
    {
        var result = await _authService.GetTwoFactorQrCodeAsync();

        if (!result.Success || result.Data == null)
            return BadRequest(result);

        return File(result.Data, "image/png");
    }

    [Authorize]
    [HttpGet("trusted-devices")]
    public async Task<IActionResult> GetTrustedDevices()
    {
        var result = await _authService.GetTrustedDevicesAsync();

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("trusted-devices/{deviceId:int}")]
    public async Task<IActionResult> RevokeTrustedDevice(int deviceId)
    {
        var result = await _authService.RevokeTrustedDeviceAsync(deviceId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("trusted-devices")]
    public async Task<IActionResult> RevokeAllTrustedDevices()
    {
        var result = await _authService.RevokeAllTrustedDevicesAsync();

        return Ok(result);
    }
}