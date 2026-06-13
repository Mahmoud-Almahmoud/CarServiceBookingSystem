using CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs;
using CarServiceBookingSystem.Application.Options;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services.BackgroundJobs
{
    public class NotificationCleanupJob : INotificationCleanupJob
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationCleanupOptions _options;
        private readonly ILogger<NotificationCleanupJob> _logger;

        public NotificationCleanupJob(
            ApplicationDbContext context,
            IOptions<NotificationCleanupOptions> options,
            ILogger<NotificationCleanupJob> logger)
        {
            _context = context;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<int> DeleteOldNotificationsAsync(
            CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled)
            {
                _logger.LogInformation("Notification cleanup job is disabled.");
                return 0;
            }

            var now = DateTime.UtcNow;

            var readCutoff = now.AddDays(-_options.ReadNotificationRetentionDays);

            var query = _context.Notifications
                .Where(x =>
                    x.IsRead &&
                    x.CreatedAt <= readCutoff);

            if (_options.DeleteOldUnreadNotifications)
            {
                var unreadCutoff = now.AddDays(-_options.UnreadNotificationRetentionDays);

                query = _context.Notifications
                    .Where(x =>
                        (x.IsRead && x.CreatedAt <= readCutoff) ||
                        (!x.IsRead && x.CreatedAt <= unreadCutoff));
            }

            var notifications = await query
                .OrderBy(x => x.Id)
                .Take(_options.BatchSize)
                .ToListAsync(cancellationToken);

            if (notifications.Count == 0)
            {
                _logger.LogInformation("Notification cleanup found no old notifications.");
                return 0;
            }

            _context.Notifications.RemoveRange(notifications);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Notification cleanup deleted {DeletedCount} old notifications.",
                notifications.Count);

            return notifications.Count;
        }
    }
}
