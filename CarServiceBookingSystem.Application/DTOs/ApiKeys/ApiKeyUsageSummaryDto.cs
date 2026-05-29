using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.ApiKeys
{
    public sealed class ApiKeyUsageSummaryDto
    {
        public int TotalKeys { get; set; }
        public int ActiveKeys { get; set; }
        public int RevokedKeys { get; set; }
        public int ExpiredKeys { get; set; }
        public int UnusedKeys { get; set; }
        public DateTime? MostRecentUsageAt { get; set; }
    }
}
