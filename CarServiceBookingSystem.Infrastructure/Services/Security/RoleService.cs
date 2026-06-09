using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Roles;
using CarServiceBookingSystem.Application.Interfaces.ISecurity;
using CarServiceBookingSystem.Application.Security;
using CarServiceBookingSystem.Infrastructure.Identity;
using MaxMind.GeoIP2.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Claims;
using System.Text;

namespace CarServiceBookingSystem.Infrastructure.Services.Security
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private static readonly HashSet<string> ProtectedRoles =
        [
            "Admin",
            "User"
        ];

        public RoleService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<ApiResponse<List<RoleResponseDto>>> GetRolesAsync()
        {
            var roles = await _roleManager.Roles
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new RoleResponseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    NormalizedName = x.NormalizedName
                })
                .ToListAsync();

            return ApiResponse<List<RoleResponseDto>>.Ok(roles);
        }

        public async Task<ApiResponse<RoleResponseDto>> GetRoleByIdAsync(string roleId)
        {
            var role = await _roleManager.Roles
                .AsNoTracking()
                .Where(x => x.Id == roleId)
                .Select(x => new RoleResponseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    NormalizedName = x.NormalizedName
                })
                .FirstOrDefaultAsync();

            if (role is null)
                return ApiResponse<RoleResponseDto>.Fail("Role not found.");

            return ApiResponse<RoleResponseDto>.Ok(role);
        }

        public async Task<ApiResponse<RolePermissionsResponse>> GetRolePermissionsAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role is null)
                return ApiResponse<RolePermissionsResponse>.Fail("Role not found.");

            var claims = await _roleManager.GetClaimsAsync(role);

            var permissions = claims
                .Where(x => x.Type == CustomClaimTypes.Permission)
                .Select(x => x.Value)
                .OrderBy(x => x)
                .ToList();

            return ApiResponse<RolePermissionsResponse>.Ok(
                new RolePermissionsResponse
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Permissions = permissions
                });
        }

        public async Task<ApiResponse<string>> AddRolePermissionAsync(string roleId,UpdateRolePermissionRequest request)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                return ApiResponse<string>.Fail("Role id is required.");

            if (string.IsNullOrWhiteSpace(request.Permission))
                return ApiResponse<string>.Fail("Permission is required.");

            var allPermissions = GetAllPermissionValues();

            if (!allPermissions.Contains(request.Permission))
            {
                return ApiResponse<string>.Fail("Invalid permission.");
            }

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role is null)
                return ApiResponse<string>.Fail("Role not found.");

            var existingClaims = await _roleManager.GetClaimsAsync(role);

            var alreadyHasPermission = existingClaims.Any(x =>
                x.Type == CustomClaimTypes.Permission &&
                x.Value == request.Permission);

            if (alreadyHasPermission)
                return ApiResponse<string>.Ok("Role already has this permission.");

            var result = await _roleManager.AddClaimAsync(
                role,
                new Claim(CustomClaimTypes.Permission, request.Permission));

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                return ApiResponse<string>.Fail($"Failed to add permission. {errors}");
            }

            return ApiResponse<string>.Ok("Permission added successfully.");
        }


        public async Task<ApiResponse<string>> RemoveRolePermissionAsync(
        string roleId,
        UpdateRolePermissionRequest request)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                return ApiResponse<string>.Fail("Role id is required.");

            if (string.IsNullOrWhiteSpace(request.Permission))
                return ApiResponse<string>.Fail("Permission is required.");

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role is null)
                return ApiResponse<string>.Fail("Role not found.");

            var existingClaims = await _roleManager.GetClaimsAsync(role);

            var claim = existingClaims.FirstOrDefault(x =>
                x.Type == CustomClaimTypes.Permission &&
                x.Value == request.Permission);

            if (claim is null)
                return ApiResponse<string>.Ok("Role does not have this permission.");

            var result = await _roleManager.RemoveClaimAsync(role, claim);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                return ApiResponse<string>.Fail($"Failed to remove permission. {errors}");
            }

            return ApiResponse<string>.Ok("Permission removed successfully.");
        }

        public async Task<ApiResponse<string>> CreateAsync(CreateRoleRequest request)
        {
            var name = request.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                return ApiResponse<string>.Fail("Role name is required.");
            }

            if (await _roleManager.RoleExistsAsync(name))
            {
                return ApiResponse<string>.Fail("Role already exists.");
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(name));

            if (!result.Succeeded)
            {
                return ApiResponse<string>.Fail(string.Join(", ", result.Errors.Select(x => x.Description)));
            }

            return ApiResponse<string>.Ok("Role created successfully.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(string roleId, UpdateRoleRequest request)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role is null)
                return ApiResponse<string>.Fail("Role not found.");

            var newName = request.Name.Trim();

            if (string.IsNullOrWhiteSpace(newName))
                return ApiResponse<string>.Fail("Role name is required.");

            if (ProtectedRoles.Contains(role.Name!))
                return ApiResponse<string>.Fail("System roles cannot be renamed.");

            var existingRole = await _roleManager.FindByNameAsync(newName);

            if (existingRole is not null && existingRole.Id != role.Id)
                return ApiResponse<string>.Fail("Role name already exists.");

            role.Name = newName;
            role.NormalizedName = _roleManager.NormalizeKey(newName);

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
                return ApiResponse<string>.Fail(string.Join(", ", result.Errors.Select(e => e.Description)));

            return ApiResponse<string>.Ok("Role updated successfully.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role is null)
                return ApiResponse<string>.Fail("Role not found.");

            if (ProtectedRoles.Contains(role.Name!))
                return ApiResponse<string>.Fail("System roles cannot be deleted.");

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);

            if (usersInRole.Any())
            {
                return ApiResponse<string>.Fail($"Cannot delete role '{role.Name}' because it is assigned to one or more users.");
            }

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
                return ApiResponse<string>.Fail(string.Join(", ", result.Errors.Select(e => e.Description)));

            return ApiResponse<string>.Ok("Role deleted successfully.");
        }

        private static HashSet<string> GetAllPermissionValues()
        {
            return typeof(Permissions)
                .GetNestedTypes()
                .SelectMany(t => t.GetFields())
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                .Select(f => f.GetRawConstantValue()?.ToString())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
    }
}
