using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.Payments;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
using CarServiceBookingSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/payments")]
[Authorize(Policy = Permissions.Payments.Manage)]
public class AdminPaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IPaymentRefundService _paymentRefundService;

    public AdminPaymentsController(IPaymentService paymentService, IPaymentRefundService paymentRefundService)
    {
        _paymentService = paymentService;
        _paymentRefundService = paymentRefundService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaymentFilterRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _paymentService.GetAllAsync(
            request,
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _paymentService.GetAdminByIdAsync(
            id,
            cancellationToken);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }


    [HttpPost("{id:int}/refund")]
    public async Task<IActionResult> RefundPayment(
    int id,
    [FromBody] RefundPaymentRequest request,
    CancellationToken cancellationToken)
    {
        var response = await _paymentRefundService.RefundPaymentAsync(
            id,
            request,
            cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }
}