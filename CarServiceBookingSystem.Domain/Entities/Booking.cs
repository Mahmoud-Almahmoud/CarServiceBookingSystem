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

        public decimal ServicePrice { get; set; }

        public decimal TravelFee { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal? CustomerLatitude { get; set; }

        public decimal? CustomerLongitude { get; set; }

        public string? CustomerCountryCode { get; set; }

        public string? CustomerCity { get; set; }
        public string? CustomerFormattedAddress { get; set; }

        public double? DistanceKm { get; set; }

        public int? EstimatedTravelTimeMinutes { get; set; }

        public int? ServicePriceRuleId { get; set; }

        public int? ServiceAreaRuleId { get; set; }

        public Payment? Payment { get; set; }
    }
}
