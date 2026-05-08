using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace CarServiceBookingSystem.Domain.Entities
{
    public class CarTrim : BaseEntity
    {
        public string Name { get; set; }

        public int YearId { get; set; }
        public CarYear Year { get; set; }

        public ICollection<Car> Cars { get; set; }
    }
}
