using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.Security
{
    public sealed class SecurityAuditTopCountryDto
    {
        public string Country { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
