using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.Roles
{
    public class RolePermissionsResponse
    {
        public string RoleId { get; set; } = null!;
        public string? RoleName { get; set; }
        public List<string> Permissions { get; set; } = [];
    }
}
