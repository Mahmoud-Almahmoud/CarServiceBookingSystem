using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Domain.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
