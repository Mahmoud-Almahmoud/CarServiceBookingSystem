using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Domain.Entities
{
    public class ServiceAreaRule : BaseIdEntity
    {
        public int? ServiceId { get; set; }

        public Service? Service { get; set; }

        public string CountryCode { get; set; } = string.Empty;

        public string? City { get; set; }

        public bool IsAllowed { get; set; }

        public int Priority { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
