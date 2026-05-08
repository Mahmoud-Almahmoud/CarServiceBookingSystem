using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Domain.Entities
{
    public class CarBrand : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<CarModel> Models { get; set; }
    }
}
