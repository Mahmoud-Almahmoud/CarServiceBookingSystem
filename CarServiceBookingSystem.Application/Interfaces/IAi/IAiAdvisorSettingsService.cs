using CarServiceBookingSystem.Application.DTOs.Ai;

namespace CarServiceBookingSystem.Application.Interfaces.IAi;

public interface IAiAdvisorSettingsService
{
    Task<AiAdvisorSettingsDto> GetAsync(
        CancellationToken cancellationToken = default);

    Task<AiAdvisorSettingsDto> UpdateAsync(
        UpdateAiAdvisorSettingsRequest request,
        CancellationToken cancellationToken = default);
}