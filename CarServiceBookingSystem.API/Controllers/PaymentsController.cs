using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.Payments;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
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
    public async Task<IActionResult> CreatePaymentIntent(CreatePaymentIntentRequest request)
    {
        var result = await _paymentService.CreatePaymentIntentAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = Permissions.Payments.View)]
    public async Task<IActionResult> GetById(
       int id,
       CancellationToken cancellationToken)
    {

        var response = await _paymentService.GetByIdAsync(
            id,
            cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpGet("booking/{bookingId:int}")]
    [Authorize(Policy = Permissions.Payments.View)]
    public async Task<IActionResult> GetByBookingId(
        int bookingId,
        CancellationToken cancellationToken)
    {

        var response = await _paymentService.GetByBookingIdAsync(
            bookingId,
            cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpGet("my")]
    [Authorize(Policy = Permissions.Payments.View)]
    public async Task<IActionResult> GetMyPayments(
        [FromQuery] PaymentFilterRequest request,
        CancellationToken cancellationToken)
    {

        var response = await _paymentService.GetMyPaymentsAsync(
            request,
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}