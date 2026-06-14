using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;

namespace CarServiceBookingSystem.Infrastructure.Services.Ai;

public sealed class AiAdvisorRuntimeSettingsProvider : IAiAdvisorRuntimeSettingsProvider
{
    private readonly IAiAdvisorSettingsRepository _settingsRepository;

    public AiAdvisorRuntimeSettingsProvider(
        IAiAdvisorSettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public Task<AiAdvisorSettingsDto> GetSettingsAsync(
        CancellationToken cancellationToken = default)
    {
        return _settingsRepository.GetAsync(cancellationToken);
    }
}