namespace CarServiceBookingSystem.Application.DTOs.Ai;

public sealed class AiAdvisorSettingsDto
{
    public bool Enabled { get; init; }

    public string Provider { get; init; } = "Ollama";

    public string BaseUrl { get; init; } = string.Empty;

    public string Model { get; init; } = string.Empty;

    public int TimeoutSeconds { get; init; }

    public int MaxSuggestions { get; init; }

    public string BookingPathTemplate { get; init; } = string.Empty;

    public int MaxPromptLength { get; init; }

    public double MinimumRecommendationConfidence { get; init; }

    public bool BlockUnrelatedQuestions { get; init; }

    public bool EnablePromptInjectionFilter { get; init; }

    public int RateLimitPerMinute { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime UpdatedAtUtc { get; init; }

    public string? UpdatedByUserId { get; init; }
}