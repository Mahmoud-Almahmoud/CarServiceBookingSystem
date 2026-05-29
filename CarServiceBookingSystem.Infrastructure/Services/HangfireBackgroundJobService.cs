using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.BackgroundJob;
using CarServiceBookingSystem.Application.Interfaces;
using Hangfire;
using Hangfire.Storage;
using MaxMind.GeoIP2.Responses;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class HangfireBackgroundJobService : IBackgroundJobService
{
    public void EnqueueEmail(string to, string subject, string body)
    {
        BackgroundJob.Enqueue<IEmailService>(
            emailService => emailService.SendAsync(to, subject, body));
    }

    public ApiResponse<BackgroundJobStatsDto> GetStatistics()
    {
        var stats = JobStorage.Current
            .GetMonitoringApi()
            .GetStatistics();

        return ApiResponse<BackgroundJobStatsDto>.Ok(
            new BackgroundJobStatsDto
            {
                Enqueued = stats.Enqueued,
                Processing = stats.Processing,
                Succeeded = stats.Succeeded,
                Failed = stats.Failed,
                Scheduled = stats.Scheduled,
                Recurring = stats.Recurring
            });
    }

    public ApiResponse<List<FailedBackgroundJobDto>> GetFailedJobs(int page, int pageSize)
    {
        if (page <= 0)
            return ApiResponse<List<FailedBackgroundJobDto>>.Fail("Page must be greater than zero.");

        if (pageSize <= 0 || pageSize > 100)
            return ApiResponse<List<FailedBackgroundJobDto>>.Fail("Page size must be between 1 and 100.");

        var from = (page - 1) * pageSize;

        var failedJobs = JobStorage.Current
            .GetMonitoringApi()
            .FailedJobs(from, pageSize);

        var result = failedJobs
            .Select(x => new FailedBackgroundJobDto
            {
                JobId = x.Key,
                JobName = x.Value.Job?.Type?.Name + "." + x.Value.Job?.Method?.Name,
                ExceptionMessage = x.Value.ExceptionMessage,
                ExceptionType = x.Value.ExceptionType,
                FailedAt = x.Value.FailedAt
            })
            .ToList();

        return ApiResponse<List<FailedBackgroundJobDto>>.Ok(result);
    }

    public ApiResponse<string> RetryFailedJob(string jobId)
    {
        if (string.IsNullOrWhiteSpace(jobId))
            return ApiResponse<string>.Fail("Job id is required.");

        var jobDetails = JobStorage.Current
            .GetMonitoringApi()
            .JobDetails(jobId);

        if (jobDetails is null)
            return ApiResponse<string>.Fail("Job not found.");

        var currentState = jobDetails.History
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault()
            ?.StateName;

        if (currentState != "Failed")
            return ApiResponse<string>.Fail("Only failed jobs can be retried.");

        var requeued = BackgroundJob.Requeue(jobId);

        if (!requeued)
            return ApiResponse<string>.Fail("Failed to retry job.");

        return ApiResponse<string>.Ok("Job retried successfully.");
    }

    public ApiResponse<string> DeleteFailedJob(string jobId)
    {
        if (string.IsNullOrWhiteSpace(jobId))
            return ApiResponse<string>.Fail("Job id is required.");

        var jobDetails = JobStorage.Current
            .GetMonitoringApi()
            .JobDetails(jobId);

        if (jobDetails is null)
            return ApiResponse<string>.Fail("Job not found.");

        var currentState = jobDetails.History
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault()
            ?.StateName;

        if (currentState != "Failed")
            return ApiResponse<string>.Fail("Only failed jobs can be deleted from this endpoint.");

        var deleted = BackgroundJob.Delete(jobId);

        if (!deleted)
            return ApiResponse<string>.Fail("Failed to delete job.");

        return ApiResponse<string>.Ok("Job deleted successfully.");
    }

    public ApiResponse<List<RecurringBackgroundJobDto>> GetRecurringJobs()
    {
        var recurringJobs = JobStorage.Current
            .GetConnection()
            .GetRecurringJobs();

        var result = recurringJobs
            .Select(x => new RecurringBackgroundJobDto
            {
                Id = x.Id,
                Cron = x.Cron,
                Queue = x.Queue,
                JobName = x.Job?.Type?.Name + "." + x.Job?.Method?.Name,
                LastExecution = x.LastExecution,
                NextExecution = x.NextExecution,
                LastJobState = x.LastJobState
            })
            .OrderBy(x => x.Id)
            .ToList();

        return ApiResponse<List<RecurringBackgroundJobDto>>.Ok(result);
    }

    public ApiResponse<string> TriggerRecurringJob(string recurringJobId)
    {
        if (string.IsNullOrWhiteSpace(recurringJobId))
            return ApiResponse<string>.Fail("Recurring job id is required.");

        var recurringJobs = JobStorage.Current
            .GetConnection()
            .GetRecurringJobs();

        var exists = recurringJobs.Any(x => x.Id == recurringJobId);

        if (!exists)
            return ApiResponse<string>.Fail("Recurring job not found.");

        RecurringJob.TriggerJob(recurringJobId);

        return ApiResponse<string>.Ok("Recurring job triggered successfully.");
    }

    public ApiResponse<RecurringJobDetailsDto> GetRecurringJob(string recurringJobId)
    {
        var job = JobStorage.Current
            .GetConnection()
            .GetRecurringJobs()
            .FirstOrDefault(x => x.Id == recurringJobId);

        if (job is null)
            return ApiResponse<RecurringJobDetailsDto>.Fail("Recurring job not found.");

        var result = new RecurringJobDetailsDto
        {
            Id = job.Id,
            Cron = job.Cron,
            Queue = job.Queue,
            JobName = $"{job.Job?.Type?.Name}.{job.Job?.Method?.Name}",
            LastExecution = job.LastExecution,
            NextExecution = job.NextExecution,
            LastJobState = job.LastJobState,
            Error = job.Error
        };

        return ApiResponse<RecurringJobDetailsDto>.Ok(result);
    }
}