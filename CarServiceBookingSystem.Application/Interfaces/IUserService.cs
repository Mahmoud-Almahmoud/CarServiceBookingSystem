using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarServiceBookingSystem.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<PagedResponse<UserResponse>>> GetUsersAsync(UserRequest query);
        Task<ApiResponse<UserResponse>> GetUserByIdAsync(string userId);
        Task<ApiResponse<string>> LockUserAsync(string userId);
        Task<ApiResponse<string>> UnlockUserAsync(string userId);
        Task<ApiResponse<string>> RevokeUserSessionsAsync(string userId);
        Task<ApiResponse<UserRolesResponse>> GetUserRolesAsync(string userId);
        Task<ApiResponse<string>> AddUserRoleAsync(string userId, UpdateUserRoleRequest request);
        Task<ApiResponse<string>> RemoveUserRoleAsync(string userId, UpdateUserRoleRequest request);
    }
}
