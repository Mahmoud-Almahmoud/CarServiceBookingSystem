using Asp.Versioning;
using CarServiceBookingSystem.API.Filters;
using CarServiceBookingSystem.Application.DTOs.Bookings;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IBookingQuoteService _bookingQuoteService;
    private readonly IBookingAvailabilityService _bookingAvailabilityService;
    private readonly IBookingAssignmentService _bookingAssignmentService;

    public BookingsController(IBookingService bookingService, 
        IBookingQuoteService bookingQuoteService, 
        IBookingAvailabilityService bookingAvailabilityService,
        IBookingAssignmentService bookingAssignmentService)
    {
        _bookingService = bookingService;
        _bookingQuoteService = bookingQuoteService;
        _bookingAvailabilityService = bookingAvailabilityService;
        _bookingAssignmentService = bookingAssignmentService;
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilter<CreateBookingRequest>))]
    [Authorize(Policy = Permissions.Bookings.Create)]
    public async Task<IActionResult> Create(CreateBookingRequest request)
    {
        var result = await _bookingService.CreateAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("my-bookings")]
    [Authorize(Policy = Permissions.Bookings.ViewMine)]
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

    [HttpPost("quote")]
    [Authorize(Policy = Permissions.Bookings.Create)]
    [ServiceFilter(typeof(ValidationFilter<BookingQuoteRequest>))]
    public async Task<IActionResult> GetQuote([FromBody] BookingQuoteRequest request,CancellationToken cancellationToken)
    {

        var response = await _bookingQuoteService.GetQuoteAsync(
            request,
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("available-slots")]
    [Authorize(Policy = Permissions.Bookings.ViewMine)]
    public async Task<IActionResult> GetAvailableSlots(
       [FromQuery] AvailableSlotsRequest request,
       CancellationToken cancellationToken)
    {
        var response = await _bookingAvailabilityService.GetAvailableSlotsAsync(
            request,
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("{id:int}/assign-technician")]
    public async Task<IActionResult> AssignTechnician(
    int id,
    [FromBody] AssignTechnicianRequest request,
    CancellationToken cancellationToken)
    {
        var response = await _bookingAssignmentService.AssignTechnicianAsync(
            id,
            request,
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("{id:int}/auto-assign-technician")]
    public async Task<IActionResult> AutoAssignTechnician(
    int id,
    CancellationToken cancellationToken)
    {
        var response = await _bookingAssignmentService.AutoAssignTechnicianAsync(
            id,
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [Authorize]
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> CancelMyBooking(
    int id,
    [FromBody] CancelBookingRequest request,
    CancellationToken cancellationToken)
    {

        var response = await _bookingService.CancelMyBookingAsync(
            id,
            request,
            cancellationToken);

        if (!response.Success)
        {
            if (response.Message == "Booking not found.")
            {
                return NotFound(response);
            }

            return BadRequest(response);
        }

        return Ok(response);
    }
}