namespace CarServiceBookingSystem.Application.DTOs.Ai;

public sealed class AiAnalyticsOverviewDto
{
    public int TotalConversations { get; init; }

    public int TotalMessages { get; init; }

    public int TotalRecommendations { get; init; }

    public int HelpfulFeedbackCount { get; init; }

    public int NotHelpfulFeedbackCount { get; init; }

    public double FeedbackRate { get; init; }

    public double AverageConfidence { get; init; }

    public int ConversationsLast7Days { get; init; }

    public int RecommendationsLast7Days { get; init; }
}