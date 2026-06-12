using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Permissions;

namespace CarServiceBookingSystem.Application.Interfaces.ISecurity
{
    public interface IPermissionService
    {
        ApiResponse<List<PermissionGroupDto>> GetAll(string? group, string? search);
    }
}
