using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.ServicePricing;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/service-price-rules")]
[Authorize(Policy = Permissions.Services.Manage)]
public class ServicePriceRulesController : ControllerBase
{
    private readonly IServicePricingService _servicePricingService;

    public ServicePriceRulesController(IServicePricingService servicePricingService)
    {
        _servicePricingService = servicePricingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRules(
        [FromQuery] ServicePriceRuleFilterRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _servicePricingService.GetRulesAsync(request, cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRuleById(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _servicePricingService.GetRuleByIdAsync(id, cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRule(
        [FromBody] CreateServicePriceRuleRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _servicePricingService.CreateRuleAsync(request, cancellationToken);

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
        [FromBody] UpdateServicePriceRuleRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _servicePricingService.UpdateRuleAsync(id, request, cancellationToken);

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
        var response = await _servicePricingService.DeleteRuleAsync(id, cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
}