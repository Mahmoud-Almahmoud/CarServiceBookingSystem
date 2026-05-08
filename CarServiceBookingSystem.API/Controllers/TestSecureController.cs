using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public class TestSecureController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("You are authorized");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult AdminOnly()
    {
        return Ok("You are admin");
    }
    [HttpGet("error-test")]
    public IActionResult ErrorTest()
    {
        throw new Exception("Test exception");
    }
}