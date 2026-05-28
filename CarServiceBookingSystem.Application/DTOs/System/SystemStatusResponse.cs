using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.System
{
    public class SystemStatusResponse
    {
        public bool DatabaseReachable { get; set; }
        public bool StripeConfigured { get; set; }
        public bool EmailConfigured { get; set; }
        public bool GeoLiteDbExists { get; set; }

        public string Environment { get; set; } = null!;
        public string ServerTimeUtc { get; set; } = null!;
        public string? AppVersion { get; set; }
    }
}
