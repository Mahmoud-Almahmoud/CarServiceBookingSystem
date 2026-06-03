using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs;
using CarServiceBookingSystem.Application.Options;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CarServiceBookingSystem.Infrastructure.Services.BackgroundJobs;

public class BookingCleanupJob : IBookingCleanupJob
{
    private readonly ApplicationDbContext _context;
    private readonly BookingCleanupOptions _options;
    private readonly ILogger<BookingCleanupJob> _logger;

    public BookingCleanupJob(
        ApplicationDbContext context,
        IOptions<BookingCleanupOptions> options,
        ILogger<BookingCleanupJob> logger)
    {
        _context = context;
        _options = options.Value;
        _logger = logger;
    }

    public async Task CancelStalePendingBookingsAsync(
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Booking cleanup job is disabled.");
            return;
        }

        var expiryMinutes = _options.PendingBookingExpiryMinutes <= 0
            ? 30
            : _options.PendingBookingExpiryMinutes;

        var batchSize = _options.BatchSize <= 0
            ? 100
            : _options.BatchSize;

        var cutoff = DateTime.UtcNow.AddMinutes(-expiryMinutes);

        var staleBookings = await _context.Bookings
            .Include(x => x.Payment)
            .Where(x =>
                x.Status == BookingStatus.Pending &&
                x.CreatedAt <= cutoff &&
                (x.Payment == null || x.Payment.Status != PaymentStatus.Succeeded))
            .OrderBy(x => x.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        if (staleBookings.Count == 0)
        {
            _logger.LogInformation("No stale pending bookings found.");
            return;
        }

        foreach (var booking in staleBookings)
        {
            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTime.UtcNow;
            booking.CancellationReason = "Automatically cancelled because payment was not completed in time.";
            booking.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Cancelled {Count} stale pending bookings older than {ExpiryMinutes} minutes.",
            staleBookings.Count,
            expiryMinutes);
    }
}