using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.Reviews;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/reviews")]
[Authorize(Policy = Permissions.Bookings.Manage)]
public class AdminBookingReviewsController : ControllerBase
{
    private readonly IBookingReviewService _bookingReviewService;

    public AdminBookingReviewsController(IBookingReviewService bookingReviewService)
    {
        _bookingReviewService = bookingReviewService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] BookingReviewQueryRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _bookingReviewService.GetAllAsync(
            request,
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _bookingReviewService.GetByIdAsync(
            id,
            cancellationToken);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpPut("{id:int}/visibility")]
    public async Task<IActionResult> UpdateVisibility(
        int id,
        [FromBody] AdminUpdateBookingReviewVisibilityRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _bookingReviewService.UpdateVisibilityAsync(
            id,
            request,
            cancellationToken);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }
}