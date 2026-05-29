using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.ApiKeys
{
    public sealed class ApiKeyUsageQuery
    {
        public bool? ActiveOnly { get; set; }
        public bool? UnusedOnly { get; set; }
        public bool? ExpiredOnly { get; set; }
    }
}
