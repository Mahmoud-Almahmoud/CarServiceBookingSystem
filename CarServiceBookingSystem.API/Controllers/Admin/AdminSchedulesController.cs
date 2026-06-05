using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.Schedules;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin")]
[Authorize(Policy = Permissions.Bookings.Manage)]
public class AdminSchedulesController : ControllerBase
{
    private readonly ITechnicianScheduleService _technicianScheduleService;

    public AdminSchedulesController(ITechnicianScheduleService technicianScheduleService)
    {
        _technicianScheduleService = technicianScheduleService;
    }

    [HttpGet("technicians/{id:int}/schedule")]
    public async Task<IActionResult> GetTechnicianSchedule(
        int id,
        [FromQuery] TechnicianScheduleQueryRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _technicianScheduleService.GetTechnicianScheduleAsync(
            id,
            request,
            cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpGet("service-branches/{id:int}/schedule")]
    public async Task<IActionResult> GetBranchSchedule(
        int id,
        [FromQuery] BranchScheduleQueryRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _technicianScheduleService.GetBranchScheduleAsync(
            id,
            request,
            cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
}