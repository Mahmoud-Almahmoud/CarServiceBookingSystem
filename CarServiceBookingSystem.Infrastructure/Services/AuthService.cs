using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Auth;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Identity;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _context = context;
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return ApiResponse<AuthResponse>.Fail("Email already exists");
        }

        var user = new ApplicationUser
        {
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return ApiResponse<AuthResponse>.Fail(
                "Registration failed",
                result.Errors.Select(x => x.Description).ToList());
        }

        await _userManager.AddToRoleAsync(user, Roles.User);

        var roles = await _userManager.GetRolesAsync(user);

        var authUser = new AuthUser
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles
        };

        var accessToken = await _tokenService.CreateAccessTokenAsync(authUser);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = _tokenService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return ApiResponse<AuthResponse>.Fail("Invalid credentials");
        }

        var validPassword = await _userManager.CheckPasswordAsync(
            user,
            request.Password);

        if (!validPassword)
        {
            return ApiResponse<AuthResponse>.Fail("Invalid credentials");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var authUser = new AuthUser
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles
        };

        var accessToken = await _tokenService.CreateAccessTokenAsync(authUser);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = _tokenService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }

    public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.Token == request.RefreshToken &&
                !x.IsRevoked &&
                x.ExpiresAt > DateTime.UtcNow);

        if (storedToken == null)
        {
            return ApiResponse<AuthResponse>.Fail("Invalid or expired refresh token");
        }

        var user = await _userManager.FindByIdAsync(storedToken.UserId);

        if (user == null)
        {
            return ApiResponse<AuthResponse>.Fail("User not found");
        }

        storedToken.IsRevoked = true;

        var roles = await _userManager.GetRolesAsync(user);

        var authUser = new AuthUser
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles
        };

        var newAccessToken = await _tokenService.CreateAccessTokenAsync(authUser);

        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = _tokenService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await _context.RefreshTokens.AddAsync(newRefreshToken);
        await _context.SaveChangesAsync();

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(60)
        }, "Token refreshed successfully");
    }

    public async Task<ApiResponse<string>> LogoutAsync(LogoutRequest request)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.Token == request.RefreshToken &&
                !x.IsRevoked);

        if (storedToken == null)
        {
            return ApiResponse<string>.Fail("Invalid refresh token");
        }

        storedToken.IsRevoked = true;

        await _context.SaveChangesAsync();

        return ApiResponse<string>.Ok("Logged out", "Logout successful");
    }
}