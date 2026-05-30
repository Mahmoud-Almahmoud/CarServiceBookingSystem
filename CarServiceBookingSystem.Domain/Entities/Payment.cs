using CarServiceBookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
        public decimal Amount { get; set; }
        public string? PaymentIntentId { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime PaidAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Currency { get; set; } = "aed";
        public string? StripeClientSecret { get; set; }
        public string? FailureReason { get; set; }
        public string? FailureCode { get; set; }
        public string? DeclineCode { get; set; }
    }
}
