
namespace CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs
{
    public interface ISecurityAuditLogCleanupService
    {
        Task CleanupOldLogsAsync();
    }
}
