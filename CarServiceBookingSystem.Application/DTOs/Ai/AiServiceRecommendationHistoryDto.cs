namespace CarServiceBookingSystem.Application.DTOs.Ai;

public sealed class AiServiceRecommendationHistoryDto
{
    public int Id { get; init; }

    public int ConversationId { get; init; }

    public int? AssistantMessageId { get; init; }

    public int ServiceId { get; init; }

    public string ServiceNameSnapshot { get; init; } = string.Empty;

    public string Reason { get; init; } = string.Empty;

    public double Confidence { get; init; }

    public string BookingUrl { get; init; } = string.Empty;

    public DateTime CreatedAtUtc { get; init; }
}