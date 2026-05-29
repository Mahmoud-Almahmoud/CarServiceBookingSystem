using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.ApiKeys
{
    public sealed class ApiKeyUsageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public string? LastUsedIpAddress { get; set; }
        public bool IsActive { get; set; }
        public bool IsExpired { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
