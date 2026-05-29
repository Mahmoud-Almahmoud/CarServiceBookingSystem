using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.Security
{
    public sealed class SecurityAuditHourlyRiskTrendDto
    {
        public DateTime HourUtc { get; set; }

        public int FailedLogins { get; set; }
        public int SuspiciousLogins { get; set; }
        public int AccessDenied { get; set; }
        public int ApiKeyValidationFailed { get; set; }

        public int TotalRiskEvents { get; set; }
    }
}
