using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class SecurityAuditLogCleanupService : ISecurityAuditLogCleanupService
{
    private readonly ApplicationDbContext _context;

    public SecurityAuditLogCleanupService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CleanupOldLogsAsync()
    {
        var cutoff = DateTime.UtcNow.AddDays(-180);

        await _context.SecurityAuditLogs
            .Where(x => x.CreatedAt < cutoff)
            .ExecuteDeleteAsync();
    }
}