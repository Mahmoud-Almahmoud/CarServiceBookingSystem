using CarServiceBookingSystem.Application.Interfaces;
using Hangfire;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class HangfireBackgroundJobService : IBackgroundJobService
{
    public void EnqueueEmail(string to, string subject, string body)
    {
        BackgroundJob.Enqueue<IEmailService>(
            emailService => emailService.SendAsync(to, subject, body));
    }
}