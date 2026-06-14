namespace CarServiceBookingSystem.Application.DTOs.Ai;

public sealed class AiSafetyCheckResult
{
    public bool IsAllowed { get; init; }

    public string? Reason { get; init; }

    public static AiSafetyCheckResult Allow()
    {
        return new AiSafetyCheckResult
        {
            IsAllowed = true
        };
    }

    public static AiSafetyCheckResult Block(string reason)
    {
        return new AiSafetyCheckResult
        {
            IsAllowed = false,
            Reason = reason
        };
    }
}