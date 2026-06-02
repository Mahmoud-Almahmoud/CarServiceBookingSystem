using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending = 1,
        Succeeded = 2,
        Failed = 3,
        Cancelled = 4,
        RefundPending = 5,
        Refunded = 6,
        RefundFailed = 7
    }
}
