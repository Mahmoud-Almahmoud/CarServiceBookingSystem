using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.ServiceBranches;
using CarServiceBookingSystem.Application.Interfaces.IServices;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/service-branches")]
[Authorize(Policy = Permissions.Services.View)]
public class ServiceBranchesController : ControllerBase
{
    private readonly IServiceBranchService _serviceBranchService;

    public ServiceBranchesController(IServiceBranchService serviceBranchService)
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

}