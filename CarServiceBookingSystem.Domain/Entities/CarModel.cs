using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Domain.Entities
{
    public class CarModel : BaseEntity
    {
        public string Name { get; set; }

        public int BrandId { get; set; }
        public CarBrand Brand { get; set; }

        public ICollection<CarYear> Years { get; set; }
    }
}
