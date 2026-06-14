using CarServiceBookingSystem.Application.DTOs.Ai;

namespace CarServiceBookingSystem.Application.Interfaces.IAi;

public interface IAiAdvisorRuntimeSettingsProvider
{
    Task<AiAdvisorSettingsDto> GetSettingsAsync(
        CancellationToken cancellationToken = default);
}