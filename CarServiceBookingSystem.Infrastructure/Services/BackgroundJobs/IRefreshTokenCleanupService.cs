using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class RefreshTokenCleanupService : IRefreshTokenCleanupService
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenCleanupService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CleanupExpiredAndOldRevokedTokensAsync()
    {
        var now = DateTime.UtcNow;
        var revokedRetentionDate = now.AddDays(-90);

        await _context.RefreshTokens
            .Where(x =>
                x.ExpiresAt < now ||
                (
                    x.RevokedAt != null &&
                    x.RevokedAt < revokedRetentionDate
                ))
            .ExecuteDeleteAsync();
    }
}