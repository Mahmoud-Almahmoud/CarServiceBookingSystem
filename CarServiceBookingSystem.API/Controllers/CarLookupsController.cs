using Asp.Versioning;
using CarServiceBookingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public class CarLookupsController : ControllerBase
{
    private readonly ICarLookupService _carLookupService;

    public CarLookupsController(ICarLookupService carLookupService)
    {
        _carLookupService = carLookupService;
    }

    [HttpGet("brands")]
    public async Task<IActionResult> GetBrands()
    {
        return Ok(await _carLookupService.GetBrandsAsync());
    }

    [HttpGet("models/{brandId:int}")]
    public async Task<IActionResult> GetModelsByBrand(int brandId)
    {
        return Ok(await _carLookupService.GetModelsByBrandAsync(brandId));
    }

    [HttpGet("years/{modelId:int}")]
    public async Task<IActionResult> GetYearsByModel(int modelId)
    {
        return Ok(await _carLookupService.GetYearsByModelAsync(modelId));
    }

    [HttpGet("trims/{yearId:int}")]
    public async Task<IActionResult> GetTrimsByYear(int yearId)
    {
        return Ok(await _carLookupService.GetTrimsByYearAsync(yearId));
    }
}