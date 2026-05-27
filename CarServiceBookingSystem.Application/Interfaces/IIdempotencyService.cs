using CarServiceBookingSystem.Application.Common.Models;

namespace CarServiceBookingSystem.Application.Common.Interfaces;

public interface IIdempotencyService
{
    Task<IdempotencyCheckResult> CheckAsync(
        string key,
        string userId,
        string endpoint,
        string requestHash,
        CancellationToken cancellationToken = default);

    Task CompleteAsync(
        Guid id,
        int statusCode,
        string responseBody,
        CancellationToken cancellationToken = default);
}