using CarServiceBookingSystem.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.Users
{
    public class UserRequest : PagedRequest
    {

        public string? Email { get; set; }
        public string? UserName { get; set; }
        public bool? IsLockedOut { get; set; }

    }
}
