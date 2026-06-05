using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.Security
{
    public sealed class MyActivityRequest : PagedRequest
    {
        public SecurityAuditEventType? EventType { get; set; }
        public DateTime? FromUtc { get; set; }
        public DateTime? ToUtc { get; set; }
    }
}
