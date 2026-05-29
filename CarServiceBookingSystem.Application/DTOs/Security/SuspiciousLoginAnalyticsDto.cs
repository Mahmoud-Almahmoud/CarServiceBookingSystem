using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.Security
{
    public sealed class SuspiciousLoginAnalyticsDto
    {
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? IpAddress { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public int Count { get; set; }
        public DateTime LastOccurredAt { get; set; }
    }
}
