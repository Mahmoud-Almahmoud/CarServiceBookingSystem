using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Healthy");
        }
    }
}
