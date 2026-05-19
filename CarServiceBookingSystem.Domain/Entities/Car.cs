using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Domain.Entities
{
    public class Car : BaseEntity
    {
        public string UserId { get; set; }

        public int CarTrimId { get; set; }
        public CarTrim CarTrim { get; set; }

        public string PlateNumber { get; set; }
    }
}
