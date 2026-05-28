using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.DTOs.System
{
    public class SystemConfigurationResponse
    {
        public string Environment { get; set; } = null!;

        public bool JwtConfigured { get; set; }
        public bool RefreshTokensEnabled { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool ApiKeysEnabled { get; set; }
        public bool StripeConfigured { get; set; }
        public bool EmailConfigured { get; set; }

        public bool SwaggerProtected { get; set; }
        public bool HangfireProtected { get; set; }
        public bool RateLimitingEnabled { get; set; }
        public bool SecurityHeadersEnabled { get; set; }
    }
}
