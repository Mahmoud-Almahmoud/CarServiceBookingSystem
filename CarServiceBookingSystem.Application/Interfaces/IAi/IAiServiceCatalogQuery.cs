using CarServiceBookingSystem.Application.DTOs.Ai;

namespace CarServiceBookingSystem.Application.Interfaces.IAi;

public interface IAiServiceCatalogQuery
{
    Task<List<AiServiceCatalogItemDto>> GetActiveServicesAsync(
        CancellationToken cancellationToken = default);
}