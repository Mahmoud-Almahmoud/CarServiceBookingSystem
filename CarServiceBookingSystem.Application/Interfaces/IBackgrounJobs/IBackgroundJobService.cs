using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.BackgroundJob;

namespace CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs;

public interface IBackgroundJobService
{
    void EnqueueEmail(string to, string subject, string body);
    ApiResponse<BackgroundJobStatsDto> GetStatistics();
    ApiResponse<List<FailedBackgroundJobDto>> GetFailedJobs(int page, int pageSize);
    ApiResponse<List<RecurringBackgroundJobDto>> GetRecurringJobs();
    ApiResponse<string> RetryFailedJob(string jobId);

    ApiResponse<string> DeleteFailedJob(string jobId);
    ApiResponse<string> TriggerRecurringJob(string recurringJobId);
    ApiResponse<RecurringJobDetailsDto> GetRecurringJob(string recurringJobId);
}