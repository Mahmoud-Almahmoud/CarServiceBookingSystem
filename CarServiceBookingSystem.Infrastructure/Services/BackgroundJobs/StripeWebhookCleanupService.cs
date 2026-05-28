using CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class StripeWebhookCleanupService : IStripeWebhookCleanupService
{
    private readonly ApplicationDbContext _context;

    public StripeWebhookCleanupService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CleanupOldWebhookEventsAsync()
    {
        var cutoff = DateTime.UtcNow.AddDays(-90);

        await _context.StripeWebhookEvents
            .Where(x =>
                x.Processed &&
                x.ProcessedAtUtc != null &&
                x.ProcessedAtUtc < cutoff)
            .ExecuteDeleteAsync();
    }
}