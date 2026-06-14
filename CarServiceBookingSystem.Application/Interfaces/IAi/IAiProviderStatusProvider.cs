using CarServiceBookingSystem.Application.DTOs.Ai;

namespace CarServiceBookingSystem.Application.Interfaces.IAi;

public interface IAiProviderStatusProvider
{
    Task<AiProviderStatusDto> GetStatusAsync(
        CancellationToken cancellationToken = default);
}