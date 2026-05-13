using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Persistence;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class SecurityAuditService : ISecurityAuditService
{
    private readonly ApplicationDbContext _context;

    public SecurityAuditService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(
        string userId,
        string eventType,
        string? ipAddress = null,
        string? device = null,
        string? details = null)
    {
        var log = new SecurityAuditLog
        {
            UserId = userId,
            EventType = eventType,
            IpAddress = ipAddress,
            Device = device,
            Details = details
        };

        await _context.SecurityAuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }
}