using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs;
using CarServiceBookingSystem.Application.DTOs.Users;
using CarServiceBookingSystem.Application.Interfaces.IUsers;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Identity;
using CarServiceBookingSystem.Infrastructure.Persistence;
using MaxMind.GeoIP2.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _context = context;
        _roleManager = roleManager;
    }

    public async Task<ApiResponse<PagedResponse<UserResponse>>> GetUsersAsync(
        UserRequest request)
    {
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 20 : request.PageSize;
        request.PageSize = request.PageSize > 100 ? 100 : request.PageSize;

        var now = DateTimeOffset.UtcNow;

        var query = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            query = query.Where(x => x.Email != null &&
                                     x.Email.Contains(request.Email));
        }

        if (!string.IsNullOrWhiteSpace(request.UserName))
        {
            query = query.Where(x => x.UserName != null &&
                                     x.UserName.Contains(request.UserName));
        }

        if (request.IsLockedOut.HasValue)
        {
            query = request.IsLockedOut.Value
                ? query.Where(x => x.LockoutEnd != null && x.LockoutEnd > now)
                : query.Where(x => x.LockoutEnd == null || x.LockoutEnd <= now);
        }

        query = request.SortBy?.ToLowerInvariant() switch
        {
            "email" => request.Desc
                ? query.OrderByDescending(x => x.Email).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Email).ThenBy(x => x.Id),

            "username" => request.Desc
                ? query.OrderByDescending(x => x.UserName).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.UserName).ThenBy(x => x.Id),

            "lockoutend" => request.Desc
                ? query.OrderByDescending(x => x.LockoutEnd).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.LockoutEnd).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.Id)
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                Email = x.Email,
                UserName = x.UserName,
                EmailConfirmed = x.EmailConfirmed,
                TwoFactorEnabled = x.TwoFactorEnabled,
                LockoutEnd = x.LockoutEnd,
                IsLockedOut = x.LockoutEnd != null && x.LockoutEnd > now,
                AccessFailedCount = x.AccessFailedCount
            })
            .ToListAsync();

        return ApiResponse<PagedResponse<UserResponse>>.Ok(
            new PagedResponse<UserResponse>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
    }

    public async Task<ApiResponse<UserResponse>> GetUserByIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<UserResponse>.Fail("User id is required.");

        var now = DateTimeOffset.UtcNow;

        var user = await _userManager.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                Email = x.Email,
                UserName = x.UserName,
                EmailConfirmed = x.EmailConfirmed,
                TwoFactorEnabled = x.TwoFactorEnabled,
                LockoutEnd = x.LockoutEnd,
                IsLockedOut = x.LockoutEnd != null && x.LockoutEnd > now,
                AccessFailedCount = x.AccessFailedCount
            })
            .FirstOrDefaultAsync();

        if (user is null)
            return ApiResponse<UserResponse>.Fail("User not found.");

        return ApiResponse<UserResponse>.Ok(user);
    }

    public async Task<ApiResponse<string>> LockUserAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<string>.Fail("User id is required.");

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return ApiResponse<string>.Fail("User not found.");

        var result = await _userManager.SetLockoutEndDateAsync(
            user,
            DateTimeOffset.UtcNow.AddYears(100));

        if (!result.Succeeded)
            return ApiResponse<string>.Fail("Failed to lock user.");

        return ApiResponse<string>.Ok("User locked successfully.");
    }

    public async Task<ApiResponse<string>> UnlockUserAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<string>.Fail("User id is required.");

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return ApiResponse<string>.Fail("User not found.");

        var result = await _userManager.SetLockoutEndDateAsync(user, null);

        if (!result.Succeeded)
            return ApiResponse<string>.Fail("Failed to unlock user.");

        await _userManager.ResetAccessFailedCountAsync(user);

        return ApiResponse<string>.Ok("User unlocked successfully.");
    }

    public async Task<ApiResponse<string>> RevokeUserSessionsAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<string>.Fail("User id is required.");

        var userExists = await _userManager.Users.AnyAsync(x => x.Id == userId);

        if (!userExists)
            return ApiResponse<string>.Fail("User not found.");

        var now = DateTime.UtcNow;

        var refreshTokens = await _context.RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                x.RevokedAt == null)
            .ToListAsync();

        foreach (var token in refreshTokens)
        {
            token.RevokedAt = now;
            token.RevocationReason = "Revoked by admin";
        }

        await _context.SaveChangesAsync();

        return ApiResponse<string>.Ok("User sessions revoked successfully.");
    }

    public async Task<ApiResponse<UserRolesResponse>> GetUserRolesAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<UserRolesResponse>.Fail("User id is required.");

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return ApiResponse<UserRolesResponse>.Fail("User not found.");

        var roles = await _userManager.GetRolesAsync(user);

        return ApiResponse<UserRolesResponse>.Ok(new UserRolesResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Roles = roles.ToList()
        });
    }

    public async Task<ApiResponse<string>> AddUserRoleAsync(
    string userId,
    UpdateUserRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RoleName))
            return ApiResponse<string>.Fail("Role name is required.");

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return ApiResponse<string>.Fail("User not found.");

        var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);

        if (!roleExists)
            return ApiResponse<string>.Fail("Role not found.");

        if (await _userManager.IsInRoleAsync(user, request.RoleName))
            return ApiResponse<string>.Ok("User already has this role.");

        var result = await _userManager.AddToRoleAsync(user, request.RoleName);

        if (!result.Succeeded)
            return ApiResponse<string>.Fail("Failed to add role.");

        return ApiResponse<string>.Ok("Role added successfully.");
    }

    public async Task<ApiResponse<string>> RemoveUserRoleAsync(
    string userId,
    UpdateUserRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RoleName))
            return ApiResponse<string>.Fail("Role name is required.");

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return ApiResponse<string>.Fail("User not found.");

        if (!await _userManager.IsInRoleAsync(user, request.RoleName))
            return ApiResponse<string>.Ok("User does not have this role.");

        if (request.RoleName == "Admin")
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            if (admins.Count <= 1 && await _userManager.IsInRoleAsync(user, "Admin"))
                return ApiResponse<string>.Fail("Cannot remove the last Admin user.");
        }

        var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);

        if (!result.Succeeded)
            return ApiResponse<string>.Fail("Failed to remove role.");

        return ApiResponse<string>.Ok("Role removed successfully.");
    }
}