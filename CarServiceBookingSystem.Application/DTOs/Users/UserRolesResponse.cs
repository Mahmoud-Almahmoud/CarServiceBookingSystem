using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.Users
{
    public class UserRolesResponse
    {
        public string UserId { get; set; } = null!;
        public string? Email { get; set; }
        public List<string> Roles { get; set; } = [];
    }
}
