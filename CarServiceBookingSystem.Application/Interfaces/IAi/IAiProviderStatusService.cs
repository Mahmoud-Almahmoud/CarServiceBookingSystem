using CarServiceBookingSystem.Application.DTOs.Ai;

namespace CarServiceBookingSystem.Application.Interfaces.IAi;

public interface IAiProviderStatusService
{
    Task<AiProviderStatusDto> GetStatusAsync(
        CancellationToken cancellationToken = default);
}