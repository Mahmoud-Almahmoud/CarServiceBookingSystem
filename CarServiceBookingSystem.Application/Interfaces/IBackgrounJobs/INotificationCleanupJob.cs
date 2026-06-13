using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs
{
    public interface INotificationCleanupJob
    {
        Task<int> DeleteOldNotificationsAsync(
            CancellationToken cancellationToken = default);
    }
}
