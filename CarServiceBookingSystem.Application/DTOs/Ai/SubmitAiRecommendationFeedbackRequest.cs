using System.ComponentModel.DataAnnotations;

namespace CarServiceBookingSystem.Application.DTOs.Ai;

public sealed class SubmitAiRecommendationFeedbackRequest
{
    [Required]
    public string FeedbackValue { get; init; } = string.Empty;

    [StringLength(1000)]
    public string? Comment { get; init; }
}