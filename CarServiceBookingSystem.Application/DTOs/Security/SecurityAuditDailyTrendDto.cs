using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.Security
{
    public sealed class SecurityAuditDailyTrendDto
    {
        public DateTime Date { get; set; }
        public int TotalEvents { get; set; }
        public int LoginSucceeded { get; set; }
        public int LoginFailed { get; set; }
        public int SuspiciousLogins { get; set; }
        public int AccessDenied { get; set; }
    }
}
