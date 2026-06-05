using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.Technicians;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/technicians")]
[Authorize(Policy = Permissions.Technicians.Manage)]
public class AdminTechniciansController : ControllerBase
{
    private readonly ITechnicianService _technicianService;

    public AdminTechniciansController(ITechnicianService technicianService)
    {
        _technicianService = technicianService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] TechnicianQueryRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _technicianService.GetAllAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _technicianService.GetByIdAsync(id, cancellationToken);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTechnicianRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _technicianService.CreateAsync(request, cancellationToken);

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
        [FromBody] UpdateTechnicianRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _technicianService.UpdateAsync(id, request, cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _technicianService.DeleteAsync(id, cancellationToken);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("{id:int}/services")]
    public async Task<IActionResult> GetServices(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _technicianService.GetServicesAsync(id, cancellationToken);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpPost("{id:int}/services")]
    public async Task<IActionResult> AddService(
        int id,
        [FromBody] AddTechnicianServiceRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _technicianService.AddServiceAsync(id, request, cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("{id:int}/services/{serviceId:int}")]
    public async Task<IActionResult> RemoveService(
        int id,
        int serviceId,
        CancellationToken cancellationToken)
    {
        var response = await _technicianService.RemoveServiceAsync(id, serviceId, cancellationToken);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("{id:int}/working-hours")]
    public async Task<IActionResult> GetWorkingHours(
    int id,
    CancellationToken cancellationToken)
    {
        var response = await _technicianService.GetWorkingHoursAsync(id, cancellationToken);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpPut("{id:int}/working-hours")]
    public async Task<IActionResult> UpdateWorkingHours(
        int id,
        [FromBody] UpdateTechnicianWorkingHoursRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _technicianService.UpdateWorkingHoursAsync(id, request, cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{id:int}/unavailable-dates")]
    public async Task<IActionResult> GetUnavailableDates(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _technicianService.GetUnavailableDatesAsync(id, cancellationToken);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("{id:int}/unavailable-dates/{unavailableDateId:int}")]
    public async Task<IActionResult> GetUnavailableDateById(
        int id,
        int unavailableDateId,
        CancellationToken cancellationToken)
    {
        var response = await _technicianService.GetUnavailableDateByIdAsync(id, unavailableDateId, cancellationToken);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpPost("{id:int}/unavailable-dates")]
    public async Task<IActionResult> CreateUnavailableDate(
        int id,
        [FromBody] CreateTechnicianUnavailableDateRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _technicianService.CreateUnavailableDateAsync(id, request, cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return CreatedAtAction(
            nameof(GetUnavailableDateById),
            new
            {
                id,
                unavailableDateId = response.Data!.Id,
                version = "1.0"
            },
            response);
    }

    [HttpPut("{id:int}/unavailable-dates/{unavailableDateId:int}")]
    public async Task<IActionResult> UpdateUnavailableDate(
        int id,
        int unavailableDateId,
        [FromBody] UpdateTechnicianUnavailableDateRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _technicianService.UpdateUnavailableDateAsync(
            id,
            unavailableDateId,
            request,
            cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("{id:int}/unavailable-dates/{unavailableDateId:int}")]
    public async Task<IActionResult> DeleteUnavailableDate(
        int id,
        int unavailableDateId,
        CancellationToken cancellationToken)
    {
        var response = await _technicianService.DeleteUnavailableDateAsync(
            id,
            unavailableDateId,
            cancellationToken);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }
}