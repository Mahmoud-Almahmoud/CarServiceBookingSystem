using CarServiceBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public int BookingId { get; set; }
        public Booking Booking { get; set; }

        public decimal Amount { get; set; }

        public string PaymentIntentId { get; set; } // Stripe reference
        public PaymentStatus Status { get; set; }
    }
}
