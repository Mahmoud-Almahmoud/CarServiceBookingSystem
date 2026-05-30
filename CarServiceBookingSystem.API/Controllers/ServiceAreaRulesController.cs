using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.ServiceAreas;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/service-area-rules")]
[Authorize(Policy = Permissions.Services.Manage)]
public class ServiceAreaRulesController : ControllerBase
{
    private readonly IServiceAreaService _serviceAreaService;

    public ServiceAreaRulesController(IServiceAreaService serviceAreaService)
    {
        _serviceAreaService = serviceAreaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRules(
        [FromQuery] ServiceAreaRuleFilterRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _serviceAreaService.GetRulesAsync(request, cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRuleById(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _serviceAreaService.GetRuleByIdAsync(id, cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRule(
        [FromBody] CreateServiceAreaRuleRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _serviceAreaService.CreateRuleAsync(request, cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(
            nameof(GetRuleById),
            new { id = response.Data!.Id, version = "1.0" },
            response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateRule(
        int id,
        [FromBody] UpdateServiceAreaRuleRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _serviceAreaService.UpdateRuleAsync(id, request, cancellationToken);

        if (!response.Success)
        {
            if (response.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(response);
            }

            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteRule(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _serviceAreaService.DeleteRuleAsync(id, cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
}