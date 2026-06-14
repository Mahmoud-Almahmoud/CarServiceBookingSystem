using CarServiceBookingSystem.Application.DTOs.Ai;

namespace CarServiceBookingSystem.Application.Interfaces.IAi;

public interface IAiAdvisorSettingsRepository
{
    Task<AiAdvisorSettingsDto> GetAsync(
        CancellationToken cancellationToken = default);

    Task<AiAdvisorSettingsDto> UpdateAsync(
        UpdateAiAdvisorSettingsRequest request,
        string? updatedByUserId,
        CancellationToken cancellationToken = default);
}