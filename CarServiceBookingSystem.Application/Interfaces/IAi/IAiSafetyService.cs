using CarServiceBookingSystem.Application.DTOs.Ai;

namespace CarServiceBookingSystem.Application.Interfaces.Ai;

public interface IAiSafetyService
{
    AiSafetyCheckResult CheckUserMessage(string message);

    string SanitizeUserMessage(string message);
}