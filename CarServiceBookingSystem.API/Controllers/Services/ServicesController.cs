using Asp.Versioning;
using CarServiceBookingSystem.API.Filters;
using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Reviews;
using CarServiceBookingSystem.Application.DTOs.Services;
using CarServiceBookingSystem.Application.Interfaces.IBookings;
using CarServiceBookingSystem.Application.Interfaces.IServices;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers.Services;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _serviceService;
    private readonly IBookingReviewService _bookingReviewService;

    public ServicesController(IServiceService serviceService, IBookingReviewService bookingReviewService)
    {
        _serviceService = serviceService;
        _bookingReviewService = bookingReviewService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
    {
        return Ok(await _serviceService.GetAllAsync(request));
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilter<CreateServiceRequest>))]
    public async Task<IActionResult> Create(CreateServiceRequest request)
    {
        var result = await _serviceService.CreateAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(ValidationFilter<UpdateServiceRequest>))]
    public async Task<IActionResult> Update(int id, UpdateServiceRequest request)
    {
        var result = await _serviceService.UpdateAsync(id, request);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _serviceService.DeleteAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}/reviews")]
    public async Task<IActionResult> GetServiceReviews(
    int id,
    [FromQuery] PublicServiceReviewQueryRequest request,
    CancellationToken cancellationToken)
    {
        var response = await _bookingReviewService.GetPublicServiceReviewsAsync(
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