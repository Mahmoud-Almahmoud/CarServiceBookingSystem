using Asp.Versioning;
using CarServiceBookingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("create-intent/{bookingId:int}")]
    public async Task<IActionResult> CreatePaymentIntent(int bookingId)
    {
        var result = await _paymentService.CreatePaymentIntentAsync(bookingId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}