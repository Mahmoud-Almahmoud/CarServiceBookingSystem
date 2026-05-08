using Asp.Versioning;
using CarServiceBookingSystem.API.Filters;
using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilter<CreateBookingRequest>))]
    public async Task<IActionResult> Create(CreateBookingRequest request)
    {
        var result = await _bookingService.CreateAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("my-bookings")]
    public async Task<IActionResult> GetMyBookings()
    {
        return Ok(await _bookingService.GetMyBookingsAsync());
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] BookingQueryRequest request)
    {
        return Ok(await _bookingService.GetAllAsync(request));
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{bookingId:int}/status")]
    [ServiceFilter(typeof(ValidationFilter<UpdateBookingStatusRequest>))]
    public async Task<IActionResult> UpdateStatus(
        int bookingId,
        UpdateBookingStatusRequest request)
    {
        var result = await _bookingService.UpdateStatusAsync(bookingId, request);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}