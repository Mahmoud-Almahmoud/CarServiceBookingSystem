using CarServiceBookingSystem.Domain.Enums;

namespace CarServiceBookingSystem.Application.Interfaces;

public interface ISecurityAuditService
{
    Task LogAsync(
        string userId,
        SecurityAuditEventType eventType,
        string? ipAddress = null,
        string? device = null,
        string? country = null,
        string? city = null,
        string? details = null);
}