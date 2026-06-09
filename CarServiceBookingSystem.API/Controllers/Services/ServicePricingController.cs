using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.ServicePricing;
using CarServiceBookingSystem.Application.Interfaces.IServices;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarServiceBookingSystem.API.Controllers.Services;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/service-pricing")]
[Authorize]
public class ServicePricingController : ControllerBase
{
    private readonly IServicePricingService _servicePricingService;

    public ServicePricingController(IServicePricingService servicePricingService)
    {
        _servicePricingService = servicePricingService;
    }

    [HttpGet("quote")]
    [Authorize(Policy = Permissions.Bookings.Create)]
    public async Task<IActionResult> GetQuote(
        [FromQuery] ServicePriceQuoteRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var response = await _servicePricingService.GetPriceQuoteAsync(
            request,
            userId,
            cancellationToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}