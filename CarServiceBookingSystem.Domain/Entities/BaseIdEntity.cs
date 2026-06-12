using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Domain.Entities
{
    public abstract class BaseIdEntity : BaseEntity
    {
        public int Id { get; set; }
    }
}
