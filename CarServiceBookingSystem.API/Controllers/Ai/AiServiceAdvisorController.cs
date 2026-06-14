using Asp.Versioning;
using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBookingSystem.API.Controllers.Ai;

[ApiController]
[Route("api/v{version:apiVersion}/ai/service-advisor")]
[ApiVersion("1.0")]
[Authorize]
public sealed class AiServiceAdvisorController : ControllerBase
{
    private readonly IAiServiceAdvisorService _advisorService;

    public AiServiceAdvisorController(IAiServiceAdvisorService advisorService)
    {
        _advisorService = advisorService;
    }

    [HttpPost("chat")]
    [ProducesResponseType(typeof(ApiResponse<ServiceAdvisorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<ServiceAdvisorResponse>>> Chat(
        [FromBody] ServiceAdvisorChatRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _advisorService.ChatAsync(
            request,
            cancellationToken);

        return Ok(ApiResponse<ServiceAdvisorResponse>.Ok(response));
    }
}