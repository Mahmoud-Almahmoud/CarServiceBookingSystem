using CarServiceBookingSystem.Application.DTOs.Ai;

namespace CarServiceBookingSystem.Application.Interfaces.IAi;

public interface IAiConversationService
{
    Task<List<AiConversationSummaryDto>> GetMyConversationsAsync(
        bool includeArchived = false,
        CancellationToken cancellationToken = default);

    Task<AiConversationDetailsDto?> GetMyConversationAsync(
        int conversationId,
        CancellationToken cancellationToken = default);

    Task<bool> ArchiveMyConversationAsync(
        int conversationId,
        CancellationToken cancellationToken = default);
}