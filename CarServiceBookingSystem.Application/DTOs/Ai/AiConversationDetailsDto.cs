namespace CarServiceBookingSystem.Application.DTOs.Ai;

public sealed class AiConversationDetailsDto
{
    public int Id { get; init; }

    public string UserId { get; init; } = string.Empty;

    public int? CarId { get; init; }

    public string? Title { get; init; }

    public bool IsArchived { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }

    public List<AiConversationMessageDto> Messages { get; init; } = [];

    public List<AiServiceRecommendationHistoryDto> Recommendations { get; init; } = [];
}