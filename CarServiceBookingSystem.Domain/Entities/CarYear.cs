using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Domain.Entities
{
    public class CarYear : BaseEntity
    {
        public int Year { get; set; }

        public int ModelId { get; set; }
        public CarModel Model { get; set; }

        public ICollection<CarTrim> Trims { get; set; }
    }
}
