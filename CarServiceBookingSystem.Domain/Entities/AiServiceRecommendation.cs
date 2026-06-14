using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Domain.Entities;

public class AiServiceRecommendation : BaseIdEntity
{
    public int ConversationId { get; set; }

    public AiConversation Conversation { get; set; } = null!;

    public int? AssistantMessageId { get; set; }

    public AiConversationMessage? AssistantMessage { get; set; }

    public int ServiceId { get; set; }

    public string ServiceNameSnapshot { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public double Confidence { get; set; }

    public string BookingUrl { get; set; } = string.Empty;

    public AiRecommendationFeedbackValue? FeedbackValue { get; set; }

    public string? FeedbackComment { get; set; }

    public DateTime? FeedbackCreatedAtUtc { get; set; }
}