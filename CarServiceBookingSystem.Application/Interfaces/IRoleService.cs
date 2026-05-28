using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Roles;

namespace CarServiceBookingSystem.Application.Interfaces
{
    public interface IRoleService
    {
        Task<ApiResponse<string>> CreateAsync(CreateRoleRequest request);
        Task<ApiResponse<string>> UpdateAsync(string roleId, UpdateRoleRequest request);
        Task<ApiResponse<string>> DeleteAsync(string roleId);
        Task<ApiResponse<List<RoleResponseDto>>> GetRolesAsync();
        Task<ApiResponse<RoleResponseDto>> GetRoleByIdAsync(string roleId);
        Task<ApiResponse<RolePermissionsResponse>> GetRolePermissionsAsync(string roleId);
        Task<ApiResponse<string>> AddRolePermissionAsync(string roleId, UpdateRolePermissionRequest request);
        Task<ApiResponse<string>> RemoveRolePermissionAsync(string roleId, UpdateRolePermissionRequest request);
    }
}
