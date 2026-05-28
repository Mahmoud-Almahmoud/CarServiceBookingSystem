
namespace CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs
{
    public interface IRefreshTokenCleanupService
    {
        Task CleanupExpiredAndOldRevokedTokensAsync();
    }
}
