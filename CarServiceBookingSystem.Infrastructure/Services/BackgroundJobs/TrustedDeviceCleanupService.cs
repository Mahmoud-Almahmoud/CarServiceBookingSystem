using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class TrustedDeviceCleanupService : ITrustedDeviceCleanupService
{
    private readonly ApplicationDbContext _context;

    public TrustedDeviceCleanupService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CleanupExpiredTrustedDevicesAsync()
    {
        var now = DateTime.UtcNow;

        await _context.TrustedDevices
            .Where(x => x.ExpiresAt < now || x.IsRevoked)
            .ExecuteDeleteAsync();
    }
}