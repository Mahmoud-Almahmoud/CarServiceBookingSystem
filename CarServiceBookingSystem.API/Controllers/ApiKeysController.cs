using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.ApiKeys;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/api-keys")]
[Authorize(Policy = Permissions.Users.Manage)]
public class ApiKeysController : ControllerBase
{
    private readonly IApiKeyService _apiKeyService;

    public ApiKeysController(IApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }

    [HttpPost]
    [Authorize(Policy = Permissions.ApiKeys.Create)]
    public async Task<IActionResult> Create(CreateApiKeyRequest request)
    {
        var result = await _apiKeyService.CreateAsync(
            request.Name,
            request.Owner,
            request.ExpiresAt);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permissions.ApiKeys.Revoke)]
    public async Task<IActionResult> Revoke(int id)
    {
        var result = await _apiKeyService.RevokeAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize(Policy = Permissions.ApiKeys.View)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _apiKeyService.GetAllAsync();
        return Ok(result);
    }
}