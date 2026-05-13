using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Auth;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Authentication;
using CarServiceBookingSystem.Infrastructure.Identity;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        ApplicationDbContext context,IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
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
        var refreshTokenU = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = TokenHasher.Hash(refreshTokenU),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = GetIpAddress(),
            Device = GetDevice()
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        var authUser = new AuthUser
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles,
            SessionId = refreshToken.Id
        };

        var accessToken = await _tokenService.CreateAccessTokenAsync(authUser);
        

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = refreshTokenU,
            SessionId = refreshToken.Id,
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

        if (await _userManager.IsLockedOutAsync(user))
        {
            return ApiResponse<AuthResponse>.Fail("Account is temporarily locked. Try again later.");
        }

        var validPassword = await _userManager.CheckPasswordAsync(user,request.Password);

        if (!validPassword)
        {
            await _userManager.AccessFailedAsync(user);
            return ApiResponse<AuthResponse>.Fail("Invalid credentials");
        }
        await _userManager.ResetAccessFailedCountAsync(user);

       

        var roles = await _userManager.GetRolesAsync(user);

        var refreshTokenU = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = TokenHasher.Hash(refreshTokenU),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = GetIpAddress(),
            Device = GetDevice()
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        var authUser = new AuthUser
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles,
            SessionId = refreshToken.Id
        };

        var accessToken = await _tokenService.CreateAccessTokenAsync(authUser);
        

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = refreshTokenU,
            SessionId = refreshToken.Id,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }

    public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var hashedToken = TokenHasher.Hash(request.RefreshToken);
        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == hashedToken);

        if (storedToken != null && storedToken.IsRevoked)
        {
            await RevokeAllUserRefreshTokensAsync(storedToken.UserId);

            return ApiResponse<AuthResponse>.Fail("Refresh token reuse detected. All sessions have been revoked.");
        }

        if (storedToken == null || storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            return ApiResponse<AuthResponse>.Fail("Invalid or expired refresh token");
        }

        var user = await _userManager.FindByIdAsync(storedToken.UserId);

        if (user == null)
        {
            return ApiResponse<AuthResponse>.Fail("User not found");
        }

        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        var refreshTokenU = _tokenService.GenerateRefreshToken();
        storedToken.ReplacedByToken = refreshTokenU;
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevokedByIp = GetIpAddress();
        storedToken.RevocationReason = "Token rotated";

        var roles = await _userManager.GetRolesAsync(user);
        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = TokenHasher.Hash(refreshTokenU),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = GetIpAddress(),
            Device = GetDevice()
        };

        await _context.RefreshTokens.AddAsync(newRefreshToken);
        await _context.SaveChangesAsync();

        var authUser = new AuthUser
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles,
            SessionId = newRefreshToken.Id
        };

        var newAccessToken = await _tokenService.CreateAccessTokenAsync(authUser);
        

        

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AccessToken = newAccessToken,
            RefreshToken = refreshTokenU,
            SessionId = newRefreshToken.Id,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(60)
        }, "Token refreshed successfully");
    }

    public async Task<ApiResponse<string>> LogoutAsync(LogoutRequest request)
    {
        var currentSessionId = GetCurrentSessionId();

        RefreshToken? storedToken = null;

        if (currentSessionId.HasValue)
        {
            storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x =>
                    x.Id == currentSessionId.Value &&
                    !x.IsRevoked);
        }

        if (storedToken == null && !string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var hashedToken = TokenHasher.Hash(request.RefreshToken);

            storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x =>
                    x.Token == hashedToken &&
                    !x.IsRevoked);
        }

        if (storedToken == null)
        {
            return ApiResponse<string>.Fail("Active session not found");
        }

        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevokedByIp = GetIpAddress();
        storedToken.RevocationReason = "Logout";

        await _context.SaveChangesAsync();

        return ApiResponse<string>.Ok("Logged out", "Logout successful");
    }

    public async Task<ApiResponse<string>> LogoutAllDevicesAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?
            .Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return ApiResponse<string>.Fail("User is not authenticated");
        }

        var tokens = await _context.RefreshTokens
            .Where(x => x.UserId == userId && !x.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = GetIpAddress();
            token.RevocationReason = "Logout from all devices";
        }

        await _context.SaveChangesAsync();

        return ApiResponse<string>.Ok("Logged out from all devices");
    }

    private async Task RevokeAllUserRefreshTokensAsync(string userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(x => x.UserId == userId && !x.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<ApiResponse<List<ActiveSessionResponse>>> GetActiveSessionsAsync()
    {
        var currentSessionId = GetCurrentSessionId();

        var userId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<List<ActiveSessionResponse>>.Fail("User is not authenticated");

        var sessions = await _context.RefreshTokens
            .AsNoTracking()
            .Where(x => x.UserId == userId && !x.IsRevoked && x.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ActiveSessionResponse
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                ExpiresAt = x.ExpiresAt,
                CreatedByIp = x.CreatedByIp,
                Device = x.Device,
                IsCurrentSession = x.Id == currentSessionId
            })
            .ToListAsync();

        return ApiResponse<List<ActiveSessionResponse>>.Ok(sessions);
    }
    public async Task<ApiResponse<string>> RevokeSessionAsync(int sessionId)
    {
        var currentSessionId = GetCurrentSessionId();

        if (currentSessionId == sessionId)
        {
            return ApiResponse<string>.Fail(
                "You cannot revoke the current session. Use logout instead.");
        }

        var userId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<string>.Fail("User is not authenticated");

        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.Id == sessionId &&
                x.UserId == userId &&
                !x.IsRevoked);

        if (token == null)
            return ApiResponse<string>.Fail("Session not found");

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = GetIpAddress();
        token.RevocationReason = "Session revoked by user";

        await _context.SaveChangesAsync();

        return ApiResponse<string>.Ok("Session revoked successfully");
    }

    private string? GetIpAddress()
    {
        return _httpContextAccessor.HttpContext?
            .Connection.RemoteIpAddress?
            .ToString();
    }

    private string? GetDevice()
    {
        return _httpContextAccessor.HttpContext?
            .Request.Headers.UserAgent
            .ToString();
    }
    private int? GetCurrentSessionId()
    {
        var value = _httpContextAccessor.HttpContext?.User?
            .FindFirst("session_id")?
            .Value;

        return int.TryParse(value, out var sessionId)
            ? sessionId
            : null;
    }
}