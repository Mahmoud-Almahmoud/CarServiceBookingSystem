using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.Security
{
    public sealed class SecurityScoreDto
    {
        public int Score { get; set; }

        public int FailedLoginsLast24Hours { get; set; }
        public int SuspiciousLoginsLast24Hours { get; set; }
        public int AccessDeniedLast24Hours { get; set; }
        public int ApiKeyValidationFailedLast24Hours { get; set; }

        public int ActiveApiKeys { get; set; }
        public int ExpiredApiKeys { get; set; }

        public int TotalUsers { get; set; }
        public int UsersWithTwoFactorEnabled { get; set; }
        public decimal TwoFactorAdoptionRate { get; set; }
    }
}
