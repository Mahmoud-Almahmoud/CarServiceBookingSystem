using Asp.Versioning;
using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CarServiceBookingSystem.API.Controllers.Ai;

[ApiController]
[Route("api/v{version:apiVersion}/ai/service-advisor")]
[ApiVersion("1.0")]
[Authorize]
public sealed class AiServiceAdvisorController : ControllerBase
{
    private readonly IAiServiceAdvisorService _advisorService;
    private readonly IAiConversationService _conversationService;

    public AiServiceAdvisorController(
        IAiServiceAdvisorService advisorService,
        IAiConversationService conversationService)
    {
        _advisorService = advisorService;
        _conversationService = conversationService;
    }

    [HttpPost("chat")]
    [EnableRateLimiting("AiServiceAdvisorPolicy")]
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

    [HttpGet("conversations")]
    [ProducesResponseType(typeof(ApiResponse<List<AiConversationSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AiConversationSummaryDto>>>> GetConversations(
    [FromQuery] bool includeArchived = false,
    CancellationToken cancellationToken = default)
    {
        var conversations = await _conversationService.GetMyConversationsAsync(
            includeArchived,
            cancellationToken);

        return Ok(ApiResponse<List<AiConversationSummaryDto>>.Ok(conversations));
    }

    [HttpGet("conversations/{conversationId:int}")]
    [ProducesResponseType(typeof(ApiResponse<AiConversationDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AiConversationDetailsDto>>> GetConversation(
        [FromRoute] int conversationId,
        CancellationToken cancellationToken)
    {
        var conversation = await _conversationService.GetMyConversationAsync(
            conversationId,
            cancellationToken);

        if (conversation is null)
            return NotFound(ApiResponse<AiConversationDetailsDto>.Fail("Conversation not found."));

        return Ok(ApiResponse<AiConversationDetailsDto>.Ok(conversation));
    }

    [HttpPatch("conversations/{conversationId:int}/archive")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> ArchiveConversation(
        [FromRoute] int conversationId,
        CancellationToken cancellationToken)
    {
        var archived = await _conversationService.ArchiveMyConversationAsync(
            conversationId,
            cancellationToken);

        if (!archived)
            return NotFound(ApiResponse<bool>.Fail("Conversation not found."));

        return Ok(ApiResponse<bool>.Ok(true));
    }
}