using CarServiceBookingSystem.Application.DTOs.Ai;

namespace CarServiceBookingSystem.Application.Interfaces.IAi;

public interface IAiServiceAdvisorService
{
    Task<ServiceAdvisorResponse> ChatAsync(
        ServiceAdvisorChatRequest request,
        CancellationToken cancellationToken = default);
}