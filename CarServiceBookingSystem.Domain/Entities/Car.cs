using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Domain.Entities
{
    public class Car : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;

        public int CarTrimId { get; set; }
        public string? CarName { get; set; } 
        public CarTrim CarTrim { get; set; } = null!;
        public string PlateNumber { get; set; } = string.Empty;
    }
}
