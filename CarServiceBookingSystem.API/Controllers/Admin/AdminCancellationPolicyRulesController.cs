using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.CancellationPolicyRules;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/cancellation-policy-rules")]
[Authorize(Policy = Permissions.Bookings.Manage)]
public class AdminCancellationPolicyRulesController : ControllerBase
{
    private readonly ICancellationPolicyRuleService _cancellationPolicyRuleService;

    public AdminCancellationPolicyRulesController(
        ICancellationPolicyRuleService cancellationPolicyRuleService)
    {
        _cancellationPolicyRuleService = cancellationPolicyRuleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] CancellationPolicyRuleQueryRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _cancellationPolicyRuleService.GetAllAsync(
            request,
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _cancellationPolicyRuleService.GetByIdAsync(
            id,
            cancellationToken);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCancellationPolicyRuleRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _cancellationPolicyRuleService.CreateAsync(
            request,
            cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Data!.Id, version = "1.0" },
            response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateCancellationPolicyRuleRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _cancellationPolicyRuleService.UpdateAsync(
            id,
            request,
            cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _cancellationPolicyRuleService.DeleteAsync(
            id,
            cancellationToken);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }
}