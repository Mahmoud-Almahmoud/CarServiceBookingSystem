using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using CarServiceBookingSystem.Application.Interfaces.IContext;

namespace CarServiceBookingSystem.Application.Services.Ai;

public sealed class AiConversationService : IAiConversationService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAiConversationRepository _conversationRepository;

    public AiConversationService(
        ICurrentUserService currentUserService,
        IAiConversationRepository conversationRepository)
    {
        _currentUserService = currentUserService;
        _conversationRepository = conversationRepository;
    }

    public async Task<List<AiConversationSummaryDto>> GetMyConversationsAsync(
        bool includeArchived = false,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return [];

        return await _conversationRepository.GetUserConversationsAsync(
            userId,
            includeArchived,
            cancellationToken);
    }

    public async Task<AiConversationDetailsDto?> GetMyConversationAsync(
        int conversationId,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return null;

        return await _conversationRepository.GetConversationDetailsAsync(
            userId,
            conversationId,
            cancellationToken);
    }

    public async Task<bool> ArchiveMyConversationAsync(
        int conversationId,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return false;

        return await _conversationRepository.ArchiveConversationAsync(
            userId,
            conversationId,
            cancellationToken);
    }
}