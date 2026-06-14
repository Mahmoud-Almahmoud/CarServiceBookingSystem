using CarServiceBookingSystem.Application.DTOs.Ai;

namespace CarServiceBookingSystem.Application.Interfaces.Ai;

public interface IAiSafetyService
{
    AiSafetyCheckResult CheckUserMessage(
        string message,
        AiAdvisorSettingsDto settings);

    string SanitizeUserMessage(
        string message,
        AiAdvisorSettingsDto settings);
}