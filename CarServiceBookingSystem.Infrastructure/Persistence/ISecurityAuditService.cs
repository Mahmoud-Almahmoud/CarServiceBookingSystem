namespace CarServiceBookingSystem.Application.Interfaces;

public interface ISecurityAuditService
{
    Task LogAsync(
        string userId,
        string eventType,
        string? ipAddress = null,
        string? device = null,
        string? details = null);
}