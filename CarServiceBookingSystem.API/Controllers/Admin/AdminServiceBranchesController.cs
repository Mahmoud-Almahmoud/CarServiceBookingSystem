using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.ServiceBranches;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/service-branches")]
[Authorize(Policy = Permissions.Services.Manage)]
public class AdminServiceBranchesController : ControllerBase
{
    private readonly IServiceBranchService _serviceBranchService;

    public AdminServiceBranchesController(IServiceBranchService serviceBranchService)
    {
        _serviceBranchService = serviceBranchService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBranches(
        [FromQuery] ServiceBranchFilterRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.GetBranchesAsync(
            request,
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBranchById(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.GetBranchByIdAsync(
            id,
            cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBranch(
        [FromBody] CreateServiceBranchRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.CreateBranchAsync(
            request,
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(
            nameof(GetBranchById),
            new { id = response.Data!.Id, version = "1.0" },
            response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateBranch(
        int id,
        [FromBody] UpdateServiceBranchRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.UpdateBranchAsync(
            id,
            request,
            cancellationToken);

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
    public async Task<IActionResult> DeleteBranch(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.DeleteBranchAsync(
            id,
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("{id:int}/services")]
    public async Task<IActionResult> GetBranchServices(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.GetBranchServicesAsync(
            id,
            cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPost("{id:int}/services")]
    public async Task<IActionResult> AddBranchService(
        int id,
        [FromBody] AddBranchServiceRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.AddBranchServiceAsync(
            id,
            request,
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete("{id:int}/services/{serviceId:int}")]
    public async Task<IActionResult> RemoveBranchService(
        int id,
        int serviceId,
        CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.RemoveBranchServiceAsync(
            id,
            serviceId,
            cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpGet("{id:int}/working-hours")]
    public async Task<IActionResult> GetWorkingHours(
    int id,
    CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.GetWorkingHoursAsync(
            id,
            cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPut("{id:int}/working-hours")]
    public async Task<IActionResult> UpdateWorkingHours(
    int id,
    [FromBody] UpdateBranchWorkingHoursRequest request,
    CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.UpdateWorkingHoursAsync(
            id,
            request,
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("{id:int}/closures")]
    public async Task<IActionResult> GetClosures(
    int id,
    [FromQuery] BranchClosureFilterRequest request,
    CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.GetClosuresAsync(
            id,
            request,
            cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpGet("{id:int}/closures/{closureId:int}")]
    public async Task<IActionResult> GetClosureById(
    int id,
    int closureId,
    CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.GetClosureByIdAsync(
            id,
            closureId,
            cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPost("{id:int}/closures")]
    public async Task<IActionResult> CreateClosure(
    int id,
    [FromBody] CreateBranchClosureRequest request,
    CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.CreateClosureAsync(
            id,
            request,
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(
            nameof(GetClosureById),
            new { id, closureId = response.Data!.Id, version = "1.0" },
            response);
    }

    [HttpPut("{id:int}/closures/{closureId:int}")]
    public async Task<IActionResult> UpdateClosure(
    int id,
    int closureId,
    [FromBody] UpdateBranchClosureRequest request,
    CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.UpdateClosureAsync(
            id,
            closureId,
            request,
            cancellationToken);

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

    [HttpDelete("{id:int}/closures/{closureId:int}")]
    public async Task<IActionResult> DeleteClosure(
    int id,
    int closureId,
    CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.DeleteClosureAsync(
            id,
            closureId,
            cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpGet("{id:int}/capacity-rules")]
    public async Task<IActionResult> GetCapacityRules(
    int id,
    [FromQuery] BranchCapacityRuleFilterRequest request,
    CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.GetCapacityRulesAsync(
            id,
            request,
            cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpGet("{id:int}/capacity-rules/{capacityRuleId:int}")]
    public async Task<IActionResult> GetCapacityRuleById(
    int id,
    int capacityRuleId,
    CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.GetCapacityRuleByIdAsync(
            id,
            capacityRuleId,
            cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPost("{id:int}/capacity-rules")]
    public async Task<IActionResult> CreateCapacityRule(
    int id,
    [FromBody] CreateBranchCapacityRuleRequest request,
    CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.CreateCapacityRuleAsync(
            id,
            request,
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(
            nameof(GetCapacityRuleById),
            new { id, capacityRuleId = response.Data!.Id, version = "1.0" },
            response);
    }

    [HttpPut("{id:int}/capacity-rules/{capacityRuleId:int}")]
    public async Task<IActionResult> UpdateCapacityRule(
    int id,
    int capacityRuleId,
    [FromBody] UpdateBranchCapacityRuleRequest request,
    CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.UpdateCapacityRuleAsync(
            id,
            capacityRuleId,
            request,
            cancellationToken);

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

    [HttpDelete("{id:int}/capacity-rules/{capacityRuleId:int}")]
    public async Task<IActionResult> DeleteCapacityRule(
    int id,
    int capacityRuleId,
    CancellationToken cancellationToken)
    {
        var response = await _serviceBranchService.DeleteCapacityRuleAsync(
            id,
            capacityRuleId,
            cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
}