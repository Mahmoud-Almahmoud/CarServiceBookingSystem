using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.Bookings
{
    public class AvailableSlotResponse
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
