using Asp.Versioning;
using CarServiceBookingSystem.API.Filters;
using CarServiceBookingSystem.Application.DTOs.Cars;
using CarServiceBookingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;

    public CarsController(ICarService carService)
    {
        _carService = carService;
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilter<CreateCarRequest>))]
    public async Task<IActionResult> AddCar(CreateCarRequest request)
    {
        var result = await _carService.AddCarAsync(request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("my-cars")]
    public async Task<IActionResult> GetMyCars()
    {
        var result = await _carService.GetMyCarsAsync();
        return Ok(result);
    }

    [HttpDelete("{carId:int}")]
    public async Task<IActionResult> DeleteCar(int carId)
    {
        var result = await _carService.DeleteCarAsync(carId);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPut("{carId:int}")]
    [ServiceFilter(typeof(ValidationFilter<UpdateCarRequest>))]
    public async Task<IActionResult> UpdateCar(
    int carId,
    UpdateCarRequest request)
    {
        var result = await _carService.UpdateCarAsync(carId, request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}