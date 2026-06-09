using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.Reviews;
using CarServiceBookingSystem.Application.Interfaces.IBookings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers.Bookings;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reviews")]
[Authorize]
public class BookingReviewsController : ControllerBase
{
    private readonly IBookingReviewService _bookingReviewService;

    public BookingReviewsController(IBookingReviewService bookingReviewService)
    {
        _bookingReviewService = bookingReviewService;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyReviews(
        [FromQuery] BookingReviewQueryRequest request,
        CancellationToken cancellationToken)
    {

        var response = await _bookingReviewService.GetMyReviewsAsync(
            request,
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("bookings/{bookingId:int}")]
    public async Task<IActionResult> CreateReview(
        int bookingId,
        [FromBody] CreateBookingReviewRequest request,
        CancellationToken cancellationToken)
    {

        var response = await _bookingReviewService.CreateMyReviewAsync(
            bookingId,
            request,
            cancellationToken);

        if (!response.Success)
        {
            if (response.Message == "Booking not found.")
                return NotFound(response);

            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateReview(
        int id,
        [FromBody] UpdateBookingReviewRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _bookingReviewService.UpdateMyReviewAsync(
            id,
            request,
            cancellationToken);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteReview(
        int id,
        CancellationToken cancellationToken)
    {

        var response = await _bookingReviewService.DeleteMyReviewAsync(
            id,
            cancellationToken);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }
}