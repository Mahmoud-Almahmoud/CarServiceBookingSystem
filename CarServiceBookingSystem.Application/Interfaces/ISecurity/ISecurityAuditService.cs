using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface ISecurityAuditService
{
    Task LogAsync(
        string userId,
        SecurityAuditEventType eventType,
        string? details = null);
}