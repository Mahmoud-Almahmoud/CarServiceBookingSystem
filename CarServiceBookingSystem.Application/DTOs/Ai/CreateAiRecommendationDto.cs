namespace CarServiceBookingSystem.Application.DTOs.Ai;

public sealed class CreateAiRecommendationDto
{
    public int ServiceId { get; init; }

    public string ServiceNameSnapshot { get; init; } = string.Empty;

    public string Reason { get; init; } = string.Empty;

    public double Confidence { get; init; }

    public string BookingUrl { get; init; } = string.Empty;
}