using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.Auth
{
    public class SecurityAuditLogRequest : PagedRequest
    {
        public string? UserId { get; set; }
        public SecurityAuditEventType? EventType { get; set; }
        public string? IpAddress { get; set; }
        public string? Country { get; set; }

        public DateTime? FromUtc { get; set; }
        public DateTime? ToUtc { get; set; }
    }
}
