using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Domain.Enums
{
    public enum BookingStatus
    {
        Pending = 1,
        Confirmed = 2,
        Assigned = 3,
        InProgress = 4,
        Completed = 5,
        Cancelled = 6
    }
}
