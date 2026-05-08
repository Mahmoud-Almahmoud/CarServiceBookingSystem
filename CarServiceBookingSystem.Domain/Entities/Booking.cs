using CarServiceBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public string UserId { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ServiceLocationType LocationType { get; set; }

        public BookingStatus Status { get; set; }

        // Navigation
        public Payment Payment { get; set; }
    }
}
