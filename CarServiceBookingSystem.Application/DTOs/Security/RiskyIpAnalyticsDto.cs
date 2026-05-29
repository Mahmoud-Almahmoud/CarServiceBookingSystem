using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.Security
{
    public sealed class RiskyIpAnalyticsDto
    {
        public string IpAddress { get; set; } = string.Empty;
        public string? Country { get; set; }
        public string? City { get; set; }

        public int FailedLogins { get; set; }
        public int SuspiciousLogins { get; set; }
        public int AccessDeniedEvents { get; set; }
        public int TotalRiskEvents { get; set; }

        public DateTime LastOccurredAt { get; set; }
    }
}
