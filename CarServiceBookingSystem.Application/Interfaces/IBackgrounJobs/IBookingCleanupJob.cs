namespace CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs
{
    public interface IBookingCleanupJob
    {
        Task CancelStalePendingBookingsAsync(CancellationToken cancellationToken = default);
    }
}
