using Asp.Versioning;
using CarServiceBookingSystem.Application.DTOs.PromoCodes;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/promo-codes")]
[Authorize(Policy = Permissions.Bookings.Manage)]
public class AdminPromoCodesController : ControllerBase
{
    private readonly IPromoCodeService _promoCodeService;

    public AdminPromoCodesController(IPromoCodeService promoCodeService)
    {
        _promoCodeService = promoCodeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PromoCodeQueryRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _promoCodeService.GetAllAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _promoCodeService.GetByIdAsync(id, cancellationToken);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePromoCodeRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _promoCodeService.CreateAsync(request, cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Data!.Id, version = "1.0" },
            response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdatePromoCodeRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _promoCodeService.UpdateAsync(id, request, cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _promoCodeService.DeleteAsync(id, cancellationToken);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }
}