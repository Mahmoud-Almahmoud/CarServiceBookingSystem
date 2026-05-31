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
}