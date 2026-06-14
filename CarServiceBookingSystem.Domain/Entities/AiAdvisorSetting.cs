namespace CarServiceBookingSystem.Domain.Entities;

public class AiAdvisorSetting
{
    public int Id { get; set; }

    public bool Enabled { get; set; } = true;

    public string Provider { get; set; } = "Ollama";

    public string BaseUrl { get; set; } = "http://localhost:11434";

    public string Model { get; set; } = "llama3.1:8b";

    public int TimeoutSeconds { get; set; } = 60;

    public int MaxSuggestions { get; set; } = 3;

    public string BookingPathTemplate { get; set; } = "/app/bookings/new?serviceId={0}";

    public int MaxPromptLength { get; set; } = 2000;

    public double MinimumRecommendationConfidence { get; set; } = 0.45;

    public bool BlockUnrelatedQuestions { get; set; } = true;

    public bool EnablePromptInjectionFilter { get; set; } = true;

    public int RateLimitPerMinute { get; set; } = 10;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public string? UpdatedByUserId { get; set; }
}