namespace CarServiceBookingSystem.Application.DTOs.Ai;

public sealed class ServiceAdvisorResponse
{
    public string Reply { get; init; } = string.Empty;

    public string Urgency { get; init; } = "Unknown";

    public bool CanRecommend { get; init; }

    public List<AiSuggestedServiceDto> SuggestedServices { get; init; } = [];

    public List<string> FollowUpQuestions { get; init; } = [];

    public string Disclaimer { get; init; } =
        "This is not a final diagnosis. A technician should inspect the vehicle.";
}

public sealed class AiSuggestedServiceDto
{
    public int ServiceId { get; init; }

    public string ServiceName { get; init; } = string.Empty;

    public string Reason { get; init; } = string.Empty;

    public double Confidence { get; init; }

    public decimal Price { get; init; }

    public int DurationInMinutes { get; init; }

    public string BookingUrl { get; init; } = string.Empty;
}