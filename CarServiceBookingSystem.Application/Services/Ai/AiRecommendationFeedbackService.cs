using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using CarServiceBookingSystem.Application.Interfaces.IContext;

namespace CarServiceBookingSystem.Application.Services.Ai;

public sealed class AiRecommendationFeedbackService : IAiRecommendationFeedbackService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAiConversationRepository _conversationRepository;

    public AiRecommendationFeedbackService(
        ICurrentUserService currentUserService,
        IAiConversationRepository conversationRepository)
    {
        _currentUserService = currentUserService;
        _conversationRepository = conversationRepository;
    }

    public async Task<AiServiceRecommendationHistoryDto?> SubmitFeedbackAsync(
        int recommendationId,
        SubmitAiRecommendationFeedbackRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return null;

        if (!IsValidFeedbackValue(request.FeedbackValue))
            throw new ArgumentException("Feedback value must be Helpful or NotHelpful.");

        return await _conversationRepository.SubmitRecommendationFeedbackAsync(
            userId,
            recommendationId,
            request.FeedbackValue,
            request.Comment,
            cancellationToken);
    }

    private static bool IsValidFeedbackValue(string value)
    {
        return value.Equals("Helpful", StringComparison.OrdinalIgnoreCase)
            || value.Equals("NotHelpful", StringComparison.OrdinalIgnoreCase);
    }
}