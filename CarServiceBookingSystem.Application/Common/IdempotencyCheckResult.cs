namespace CarServiceBookingSystem.Application.Common.Models;

public sealed record IdempotencyCheckResult(
    bool IsReplay,
    bool IsConflict,
    Guid? RecordId,
    int? StatusCode,
    string? ResponseBody);