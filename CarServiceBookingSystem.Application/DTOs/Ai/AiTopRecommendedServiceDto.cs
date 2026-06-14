namespace CarServiceBookingSystem.Application.DTOs.Ai;

public sealed class AiTopRecommendedServiceDto
{
    public int ServiceId { get; init; }

    public string ServiceNameSnapshot { get; init; } = string.Empty;

    public int RecommendationCount { get; init; }

    public double AverageConfidence { get; init; }

    public int HelpfulCount { get; init; }

    public int NotHelpfulCount { get; init; }
}