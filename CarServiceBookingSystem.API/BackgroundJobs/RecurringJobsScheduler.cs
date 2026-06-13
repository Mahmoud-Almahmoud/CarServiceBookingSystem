using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs;
using Hangfire;

namespace CarServiceBookingSystem.API.BackgroundJobs
{
    public static class RecurringJobsScheduler
    {
        public static void RegisterRecurringJobs()
        {
            RecurringJob.AddOrUpdate<ITrustedDeviceCleanupService>(
            "cleanup-expired-trusted-devices",
            service => service.CleanupExpiredTrustedDevicesAsync(),
            Cron.Daily);

            RecurringJob.AddOrUpdate<IIdempotencyCleanupService>(
            "cleanup-expired-idempotency-keys",
            service => service.CleanupExpiredKeysAsync(),
            Cron.Daily);

            RecurringJob.AddOrUpdate<IRefreshTokenCleanupService>(
            "cleanup-refresh-tokens",
            service => service.CleanupExpiredAndOldRevokedTokensAsync(),
            Cron.Daily);

            RecurringJob.AddOrUpdate<ISecurityAuditLogCleanupService>(
            "cleanup-security-audit-logs",
            service => service.CleanupOldLogsAsync(),
            Cron.Weekly);

            RecurringJob.AddOrUpdate<IStripeWebhookCleanupService>(
            "cleanup-stripe-webhook-events",
            service => service.CleanupOldWebhookEventsAsync(),
            Cron.Weekly);

            RecurringJob.AddOrUpdate<IBookingCleanupJob>(
            "cancel-stale-pending-bookings",
            service => service.CancelStalePendingBookingsAsync(CancellationToken.None),
            Cron.MinuteInterval(5));

            RecurringJob.AddOrUpdate<INotificationCleanupJob>(
            "delete-old-notifications",
            job => job.DeleteOldNotificationsAsync(CancellationToken.None),
            Cron.Daily(3));
        }
    }
}
