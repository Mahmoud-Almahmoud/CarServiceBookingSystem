using CarServiceBookingSystem.Application.DTOs.Ai;

namespace CarServiceBookingSystem.Application.Interfaces.IAi;

public interface IAiRecommendationFeedbackService
{
    Task<AiServiceRecommendationHistoryDto?> SubmitFeedbackAsync(
        int recommendationId,
        SubmitAiRecommendationFeedbackRequest request,
        CancellationToken cancellationToken = default);
}