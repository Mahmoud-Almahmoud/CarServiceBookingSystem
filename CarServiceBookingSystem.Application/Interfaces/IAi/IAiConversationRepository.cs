using CarServiceBookingSystem.Application.DTOs.Ai;

namespace CarServiceBookingSystem.Application.Interfaces.IAi;

public interface IAiConversationRepository
{
    Task<AiConversationDto> GetOrCreateConversationAsync(
        string userId,
        int? conversationId,
        int? carId,
        string firstUserMessage,
        CancellationToken cancellationToken = default);

    Task<AiConversationMessageDto> AddMessageAsync(
        int conversationId,
        string role,
        string content,
        CancellationToken cancellationToken = default);

    Task<List<AiConversationMessageDto>> GetRecentMessagesAsync(
        int conversationId,
        int take = 10,
        CancellationToken cancellationToken = default);

    Task AddRecommendationsAsync(
        int conversationId,
        int assistantMessageId,
        List<CreateAiRecommendationDto> recommendations,
        CancellationToken cancellationToken = default);

    Task<List<AiConversationSummaryDto>> GetUserConversationsAsync(
    string userId,
    bool includeArchived = false,
    CancellationToken cancellationToken = default);

    Task<AiConversationDetailsDto?> GetConversationDetailsAsync(
        string userId,
        int conversationId,
        CancellationToken cancellationToken = default);

    Task<bool> ArchiveConversationAsync(
        string userId,
        int conversationId,
        CancellationToken cancellationToken = default);
}