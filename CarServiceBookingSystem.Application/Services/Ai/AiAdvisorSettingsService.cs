using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;
using CarServiceBookingSystem.Application.Interfaces.IContext;

namespace CarServiceBookingSystem.Application.Services.Ai;

public sealed class AiAdvisorSettingsService : IAiAdvisorSettingsService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAiAdvisorSettingsRepository _settingsRepository;

    public AiAdvisorSettingsService(
        ICurrentUserService currentUserService,
        IAiAdvisorSettingsRepository settingsRepository)
    {
        _currentUserService = currentUserService;
        _settingsRepository = settingsRepository;
    }

    public Task<AiAdvisorSettingsDto> GetAsync(
        CancellationToken cancellationToken = default)
    {
        return _settingsRepository.GetAsync(cancellationToken);
    }

    public Task<AiAdvisorSettingsDto> UpdateAsync(
        UpdateAiAdvisorSettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        return _settingsRepository.UpdateAsync(
            request,
            _currentUserService.UserId,
            cancellationToken);
    }
}