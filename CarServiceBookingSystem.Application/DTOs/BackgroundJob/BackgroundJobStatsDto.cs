using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.BackgroundJob
{
    public sealed class BackgroundJobStatsDto
    {
        public long Enqueued { get; set; }
        public long Processing { get; set; }
        public long Succeeded { get; set; }
        public long Failed { get; set; }
        public long Scheduled { get; set; }
        public long Recurring { get; set; }
    }
}
