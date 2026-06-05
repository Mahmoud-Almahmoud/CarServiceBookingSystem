using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.CancellationPolicyRules;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/bookings")]
[Authorize(Policy = Permissions.Bookings.Manage)]
public class AdminBookingsController : ControllerBase
{
    private readonly IBookingReceiptService _bookingReceiptService;

    public AdminBookingsController(
        IBookingReceiptService bookingReceiptService)
    {
        _bookingReceiptService = bookingReceiptService;
    }

    [HttpGet("{id:int}/receipt")]
    public async Task<IActionResult> GetReceipt(
    int id,
    CancellationToken cancellationToken)
    {
        var response = await _bookingReceiptService.GetAdminReceiptAsync(
            id,
            cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

}