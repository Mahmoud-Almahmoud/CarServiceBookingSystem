using CarServiceBookingSystem.Application.DTOs.Ai;
using CarServiceBookingSystem.Application.Interfaces.IAi;

namespace CarServiceBookingSystem.Application.Services.Ai;

public sealed class AiProviderStatusService : IAiProviderStatusService
{
    private readonly IAiProviderStatusProvider _statusProvider;

    public AiProviderStatusService(IAiProviderStatusProvider statusProvider)
    {
        _statusProvider = statusProvider;
    }

    public Task<AiProviderStatusDto> GetStatusAsync(
        CancellationToken cancellationToken = default)
    {
        return _statusProvider.GetStatusAsync(cancellationToken);
    }
}