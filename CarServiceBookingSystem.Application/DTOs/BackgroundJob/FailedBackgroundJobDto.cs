using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.BackgroundJob
{
    public sealed class FailedBackgroundJobDto
    {
        public string JobId { get; set; } = string.Empty;
        public string JobName { get; set; } = string.Empty;
        public string? ExceptionMessage { get; set; }
        public string? ExceptionType { get; set; }
        public DateTime? FailedAt { get; set; }
    }
}
