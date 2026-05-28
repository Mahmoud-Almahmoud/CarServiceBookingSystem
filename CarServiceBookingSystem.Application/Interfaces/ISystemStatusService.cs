using CarServiceBookingSystem.Application.DTOs.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.Interfaces
{
    public interface ISystemStatusService
    {
        Task<SystemStatusResponse> GetStatusAsync();
        SystemConfigurationResponse GetConfiguration();
    }
}
