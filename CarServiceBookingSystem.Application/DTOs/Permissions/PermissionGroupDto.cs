using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.Permissions
{
    public sealed class PermissionGroupDto
    {
        public string Group { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = [];
    }
}
