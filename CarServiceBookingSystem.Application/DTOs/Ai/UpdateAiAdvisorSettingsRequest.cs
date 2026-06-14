using System.ComponentModel.DataAnnotations;

namespace CarServiceBookingSystem.Application.DTOs.Ai;

public sealed class UpdateAiAdvisorSettingsRequest
{
    public bool Enabled { get; init; }

    [Required]
    [StringLength(50)]
    public string Provider { get; init; } = "Ollama";

    [Required]
    [StringLength(500)]
    public string BaseUrl { get; init; } = "http://localhost:11434";

    [Required]
    [StringLength(100)]
    public string Model { get; init; } = "llama3.1:8b";

    [Range(5, 300)]
    public int TimeoutSeconds { get; init; } = 60;

    [Range(1, 10)]
    public int MaxSuggestions { get; init; } = 3;

    [Required]
    [StringLength(500)]
    public string BookingPathTemplate { get; init; } = "/app/bookings/new?serviceId={0}";

    [Range(100, 8000)]
    public int MaxPromptLength { get; init; } = 2000;

    [Range(0.0, 1.0)]
    public double MinimumRecommendationConfidence { get; init; } = 0.45;

    public bool BlockUnrelatedQuestions { get; init; } = true;

    public bool EnablePromptInjectionFilter { get; init; } = true;

    [Range(1, 100)]
    public int RateLimitPerMinute { get; init; } = 10;
}