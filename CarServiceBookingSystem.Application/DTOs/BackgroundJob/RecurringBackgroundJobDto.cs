using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.BackgroundJob
{
    public sealed class RecurringBackgroundJobDto
    {
        public string Id { get; set; } = string.Empty;
        public string Cron { get; set; } = string.Empty;
        public string? Queue { get; set; }
        public string? JobName { get; set; }
        public DateTime? LastExecution { get; set; }
        public DateTime? NextExecution { get; set; }
        public string? LastJobState { get; set; }
    }
}
