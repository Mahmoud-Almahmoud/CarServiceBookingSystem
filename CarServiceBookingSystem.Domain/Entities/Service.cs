using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Domain.Entities
{
    public class Service : BaseEntity
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int DurationInMinutes { get; set; }
    }
}
