namespace CarServiceBookingSystem.Application.DTOs.Ai;

public sealed class AiDailyUsageDto
{
    public DateOnly Date { get; init; }

    public int Conversations { get; init; }

    public int Messages { get; init; }

    public int Recommendations { get; init; }
}