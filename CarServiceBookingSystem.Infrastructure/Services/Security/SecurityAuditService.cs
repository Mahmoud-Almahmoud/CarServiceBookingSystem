using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;

namespace CarServiceBookingSystem.Infrastructure.Services.Security;

public class SecurityAuditService : ISecurityAuditService
{
    private readonly ApplicationDbContext _context;

    public SecurityAuditService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(
    string userId,
    SecurityAuditEventType eventType,
    string? ipAddress = null,
    string? device = null,
    string? country = null,
    string? city = null,
        string? details = null)
    {
        var log = new SecurityAuditLog
        {
            UserId = userId,
            EventType = eventType,
            IpAddress = ipAddress,
            Device = device,
            Details = details,
            Country = country,
            City = city
        };

        await _context.SecurityAuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }
}