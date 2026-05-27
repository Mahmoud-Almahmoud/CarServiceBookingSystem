using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class IdempotencyCleanupService : IIdempotencyCleanupService
{
    private readonly ApplicationDbContext _dbContext;

    public IdempotencyCleanupService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task DeleteExpiredAsync()
    {
        var now = DateTime.UtcNow;

        await _dbContext.IdempotencyKeys
            .Where(x => x.ExpiresAtUtc <= now)
            .ExecuteDeleteAsync();
    }
}