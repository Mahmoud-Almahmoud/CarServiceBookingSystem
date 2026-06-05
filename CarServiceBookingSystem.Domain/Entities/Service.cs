using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Domain.Entities
{
    public class Service : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;

        public int DurationInMinutes { get; set; }

        public ICollection<ServicePriceRule> PriceRules { get; set; } = new List<ServicePriceRule>();
    }
}
