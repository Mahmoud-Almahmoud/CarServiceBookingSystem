using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Auth;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Security;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Authentication;
using CarServiceBookingSystem.Infrastructure.Identity;
using CarServiceBookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CarServiceBookingSystem.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly ISecurityAuditService _securityAuditService;
    private readonly IQrCodeService _qrCodeService;
    private readonly IGeoLocationService _geoLocationService;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        ApplicationDbContext context,IHttpContextAccessor httpContextAccessor,
        IBackgroundJobService backgroundJobService,
        ISecurityAuditService securityAuditService,
        IQrCodeService qrCodeService,
        IGeoLocationService geoLocationService, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _qrCodeService = qrCodeService;
        _backgroundJobService = backgroundJobService;
        _securityAuditService = securityAuditService;
        _geoLocationService = geoLocationService;
        _roleManager = roleManager;
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
            Device = GetDevice(),
            DeviceFingerprintHash = GetDeviceFingerprintHash(),
            TokenFamilyId = Guid.NewGuid().ToString()
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
        var permissions = await GetUserPermissionsAsync(user);

        var authUser = new AuthUser
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles,
            Permissions = permissions,
            SessionId = refreshToken.Id
        };

        var accessToken = await _tokenService.CreateAccessTokenAsync(authUser);

        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = $"https://your-frontend-domain.com/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(emailConfirmationToken)}";
        _backgroundJobService.EnqueueEmail(user.Email!,"Confirm your email",$"Please confirm your email by clicking this link: {confirmationLink}");


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

    public async Task<ApiResponse<string>> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return ApiResponse<string>.Fail("User not found");
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
        {
            return ApiResponse<string>.Fail(
                "Email confirmation failed",
                result.Errors.Select(x => x.Description).ToList());
        }

        return ApiResponse<string>.Ok("Email confirmed successfully");
    }
    public async Task<ApiResponse<string>> ResendEmailConfirmationAsync(
    ResendEmailConfirmationRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return ApiResponse<string>.Fail("User not found");

        if (await _userManager.IsEmailConfirmedAsync(user))
            return ApiResponse<string>.Fail("Email is already confirmed");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmationLink =$"https://your-frontend-domain.com/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
        _backgroundJobService.EnqueueEmail(user.Email!, "Confirm your email", $"Please confirm your email by clicking this link: {confirmationLink}");

        return ApiResponse<string>.Ok("Confirmation email sent successfully");
    }
    public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return ApiResponse<string>.Ok("If the email exists, a reset password link has been sent.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var resetLink =
            $"https://your-frontend-domain.com/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(token)}";

        _backgroundJobService.EnqueueEmail(user.Email!,"Reset your password",$"Reset your password using this link: {resetLink}");

        return ApiResponse<string>.Ok("If the email exists, a reset password link has been sent.");
    }
    public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return ApiResponse<string>.Fail("Invalid reset request");
        }

        var result = await _userManager.ResetPasswordAsync(
            user,
            request.Token,
            request.NewPassword);

        if (!result.Succeeded)
        {
            return ApiResponse<string>.Fail(
                "Password reset failed",
                result.Errors.Select(x => x.Description).ToList());
        }

        return ApiResponse<string>.Ok("Password reset successfully");
    }

    public async Task<ApiResponse<string>> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var userId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<string>.Fail("User is not authenticated");

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return ApiResponse<string>.Fail("User not found");

        var result = await _userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        if (!result.Succeeded)
        {
            return ApiResponse<string>.Fail(
                "Password change failed",
                result.Errors.Select(x => x.Description).ToList());
        }

        var trustedDevices = await _context.TrustedDevices.Where(x =>x.UserId == user.Id &&!x.IsRevoked).ToListAsync();
        foreach (var device in trustedDevices)
        {
            device.IsRevoked = true;
        }
        var geo = await GetGeoLocationAsync();
        await _securityAuditService.LogAsync(user.Id,"TrustedDevicesRevokedAfterPasswordChange",GetIpAddress(),GetDevice(), geo.Country, geo.City);

        var currentSessionId = GetCurrentSessionId();
        var activeTokens = await _context.RefreshTokens
            .Where(x =>x.UserId == user.Id && !x.IsRevoked && (!currentSessionId.HasValue || x.Id != currentSessionId.Value))
            .ToListAsync();

        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = GetIpAddress();
            token.RevocationReason = "Password changed";
        }
        await _context.SaveChangesAsync();

        _backgroundJobService.EnqueueEmail(user.Email!, "Password Changed",
            "Your password was changed successfully. If this was not you, please contact support immediately.");
        await _securityAuditService.LogAsync(user.Id,"PasswordChanged",GetIpAddress(),GetDevice(), geo.Country, geo.City);
        return ApiResponse<string>.Ok("Password changed successfully");
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var geo = await GetGeoLocationAsync();

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return ApiResponse<AuthResponse>.Fail("Invalid credentials");
        }
        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            return ApiResponse<AuthResponse>.Fail("Please confirm your email before logging in.");
        }
        if (await _userManager.IsLockedOutAsync(user))
        {
            return ApiResponse<AuthResponse>.Fail("Account is temporarily locked. Try again later.");
        }

        var validPassword = await _userManager.CheckPasswordAsync(user,request.Password);

        if (!validPassword)
        {
            await _userManager.AccessFailedAsync(user);
            await _securityAuditService.LogAsync(user.Id,"LoginFailed", GetIpAddress(), GetDevice(),"Invalid password", geo.Country, geo.City);
            return ApiResponse<AuthResponse>.Fail("Invalid credentials");
        }
        await _userManager.ResetAccessFailedCountAsync(user);

        var isSuspicious = await IsSuspiciousLoginAsync(user.Id);

        if (isSuspicious)
        {
            await _securityAuditService.LogAsync(
                user.Id,
                "SuspiciousLoginDetected",
                GetIpAddress(),
                GetDevice(),
                "Login from a new IP address or device", geo.Country, geo.City);

            _backgroundJobService.EnqueueEmail(user.Email!, "New Login Detected",
                "A new login was detected on your account. If this was not you, please change your password immediately.");
        }

        if (await _userManager.GetTwoFactorEnabledAsync(user))
        {
            var isTrusted = false;

            if (!string.IsNullOrWhiteSpace(request.TrustedDeviceToken))
            {
                isTrusted = await IsTrustedDeviceAsync(
                    user.Id,
                    request.TrustedDeviceToken);
            }

            if (!isTrusted)
            {
                return ApiResponse<AuthResponse>.Ok(new AuthResponse
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    RequiresTwoFactor = true
                }, "Two-factor authentication required");
            }
        }

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
            Device = GetDevice(),
            DeviceFingerprintHash = GetDeviceFingerprintHash(),
            TokenFamilyId = Guid.NewGuid().ToString()
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
        var permissions = await GetUserPermissionsAsync(user);

        var authUser = new AuthUser
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles,
            Permissions = permissions,
            SessionId = refreshToken.Id
        };

        var accessToken = await _tokenService.CreateAccessTokenAsync(authUser);
        await _securityAuditService.LogAsync(user.Id, "LoginSuccess", GetIpAddress(), GetDevice(), geo.Country, geo.City);


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
        var geo = await GetGeoLocationAsync();

        if (storedToken != null && storedToken.IsRevoked)
        {
            await RevokeRefreshTokenFamilyAsync(storedToken.TokenFamilyId,"Refresh token reuse detected");
            await _securityAuditService.LogAsync(storedToken.UserId,"RefreshTokenReuseDetected",GetIpAddress(),GetDevice(), geo.Country, geo.City);
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

        var currentFingerprintHash = GetDeviceFingerprintHash();

        if (!string.IsNullOrWhiteSpace(storedToken.DeviceFingerprintHash) &&
            storedToken.DeviceFingerprintHash != currentFingerprintHash)
        {
            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.RevokedByIp = GetIpAddress();
            storedToken.RevocationReason = "Device fingerprint mismatch";

            await _context.SaveChangesAsync();

            await _securityAuditService.LogAsync(
                storedToken.UserId,
                "RefreshTokenFingerprintMismatch",
                GetIpAddress(),
                GetDevice(),
                "Refresh token used from a different device fingerprint",
                geo.Country,
                geo.City);

            return ApiResponse<AuthResponse>.Fail(
                "Refresh token is no longer valid from this device.");
        }

        
        var refreshTokenU = _tokenService.GenerateRefreshToken();
        

        var roles = await _userManager.GetRolesAsync(user);
        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = TokenHasher.Hash(refreshTokenU),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = GetIpAddress(),
            Device = GetDevice(),
            DeviceFingerprintHash = GetDeviceFingerprintHash(),
            TokenFamilyId = storedToken.TokenFamilyId
        };

        await _context.RefreshTokens.AddAsync(newRefreshToken);
        await _context.SaveChangesAsync();

        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.ReplacedByToken = refreshTokenU;
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevokedByIp = GetIpAddress();
        storedToken.RevocationReason = "Token rotated";
        storedToken.ReplacedByTokenId = newRefreshToken.Id;

        await _context.SaveChangesAsync();
        var permissions = await GetUserPermissionsAsync(user);

        var authUser = new AuthUser
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles,
            Permissions = permissions,
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
        var geo = await GetGeoLocationAsync();

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
        await _securityAuditService.LogAsync(storedToken.UserId, "Logout", GetIpAddress(), GetDevice(), geo.Country, geo.City);
        return ApiResponse<string>.Ok("Logged out", "Logout successful");
    }

    public async Task<ApiResponse<string>> LogoutAllDevicesAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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

    public async Task<ApiResponse<EnableTwoFactorResponse>> GetTwoFactorSetupAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<EnableTwoFactorResponse>.Fail("User is not authenticated");

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return ApiResponse<EnableTwoFactorResponse>.Fail("User not found");

        var key = await _userManager.GetAuthenticatorKeyAsync(user);

        if (string.IsNullOrWhiteSpace(key))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            key = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        var response = new EnableTwoFactorResponse
        {
            SharedKey = key!,
            AuthenticatorUri = GenerateQrCodeUri(user.Email!, key!)
        };

        return ApiResponse<EnableTwoFactorResponse>.Ok(response);
    }

    public async Task<ApiResponse<string>> EnableTwoFactorAsync(VerifyTwoFactorRequest request)
    {
        var userId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<string>.Fail("User is not authenticated");

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return ApiResponse<string>.Fail("User not found");

        var code = request.Code.Replace(" ", string.Empty)
            .Replace("-", string.Empty);

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(
            user,
            _userManager.Options.Tokens.AuthenticatorTokenProvider,
            code);

        if (!isValid)
            return ApiResponse<string>.Fail("Invalid verification code");

        await _userManager.SetTwoFactorEnabledAsync(user, true);

        return ApiResponse<string>.Ok("Two-factor authentication enabled successfully");
    }
    public async Task<ApiResponse<AuthResponse>> LoginWithTwoFactorAsync(LoginTwoFactorRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return ApiResponse<AuthResponse>.Fail("Invalid credentials");

        if (!await _userManager.GetTwoFactorEnabledAsync(user))
            return ApiResponse<AuthResponse>.Fail("Two-factor authentication is not enabled");

        var code = request.Code.Replace(" ", string.Empty)
            .Replace("-", string.Empty);

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(
            user,
            _userManager.Options.Tokens.AuthenticatorTokenProvider,
            code);

        if (!isValid)
            return ApiResponse<AuthResponse>.Fail("Invalid verification code");

        string? trustedDeviceToken = null;

        if (request.RememberDevice)
        {
            trustedDeviceToken = await CreateTrustedDeviceAsync(user);
        }

        if (request.RememberDevice)
        {
            trustedDeviceToken = await CreateTrustedDeviceAsync(user);
        }

        var rawRefreshToken = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = TokenHasher.Hash(rawRefreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = GetIpAddress(),
            Device = GetDevice(),
            DeviceFingerprintHash = GetDeviceFingerprintHash(),
            TokenFamilyId = Guid.NewGuid().ToString()
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await GetUserPermissionsAsync(user);

        var authUser = new AuthUser
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles,
            Permissions = permissions,
            SessionId = refreshToken.Id
        };

        var accessToken = await _tokenService.CreateAccessTokenAsync(authUser);
        var geo = await GetGeoLocationAsync();
        await _securityAuditService.LogAsync(
            user.Id,
            "TwoFactorLoginSuccess",
            GetIpAddress(),
            GetDevice(), geo.Country, geo.City);

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken,
            SessionId = refreshToken.Id,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(60),
            TrustedDeviceToken = trustedDeviceToken
        }, "Login successful");
    }

    public async Task<ApiResponse<string>> DisableTwoFactorAsync(DisableTwoFactorRequest request)
    {
        var userId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<string>.Fail("User is not authenticated");

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return ApiResponse<string>.Fail("User not found");

        if (!await _userManager.GetTwoFactorEnabledAsync(user))
            return ApiResponse<string>.Fail("Two-factor authentication is not enabled");

        var code = request.Code.Replace(" ", string.Empty)
            .Replace("-", string.Empty);

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(
            user,
            _userManager.Options.Tokens.AuthenticatorTokenProvider,
            code);

        if (!isValid)
            return ApiResponse<string>.Fail("Invalid verification code");

        await _userManager.SetTwoFactorEnabledAsync(user, false);
        var geo = await GetGeoLocationAsync();
        await _securityAuditService.LogAsync(
            user.Id,
            "TwoFactorDisabled",
            GetIpAddress(),
            GetDevice(), geo.Country, geo.City);

        return ApiResponse<string>.Ok("Two-factor authentication disabled successfully");
    }

    public async Task<ApiResponse<RecoveryCodesResponse>> GenerateRecoveryCodesAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<RecoveryCodesResponse>
                .Fail("User is not authenticated");

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return ApiResponse<RecoveryCodesResponse>
                .Fail("User not found");

        if (!await _userManager.GetTwoFactorEnabledAsync(user))
        {
            return ApiResponse<RecoveryCodesResponse>
                .Fail("Two-factor authentication is not enabled");
        }

        var codes = await _userManager
            .GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        var geo = await GetGeoLocationAsync();
        await _securityAuditService.LogAsync(
            user.Id,
            "RecoveryCodesGenerated",
            GetIpAddress(),
            GetDevice(), geo.Country, geo.City);

        return ApiResponse<RecoveryCodesResponse>.Ok(
            new RecoveryCodesResponse
            {
                RecoveryCodes = codes.ToList()
            });
    }

    public async Task<ApiResponse<AuthResponse>> LoginWithRecoveryCodeAsync(LoginRecoveryCodeRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return ApiResponse<AuthResponse>
                .Fail("Invalid credentials");

        var result = await _userManager
            .RedeemTwoFactorRecoveryCodeAsync(user, request.RecoveryCode);

        if (!result.Succeeded)
        {
            return ApiResponse<AuthResponse>
                .Fail("Invalid recovery code");
        }

        var rawRefreshToken = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = TokenHasher.Hash(rawRefreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = GetIpAddress(),
            Device = GetDevice(),
            DeviceFingerprintHash = GetDeviceFingerprintHash(),
            TokenFamilyId = Guid.NewGuid().ToString()
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await GetUserPermissionsAsync(user);
        var authUser = new AuthUser
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles,
            Permissions = permissions,
            SessionId = refreshToken.Id
        };

        var accessToken = await _tokenService.CreateAccessTokenAsync(authUser);
        var geo = await GetGeoLocationAsync();
        await _securityAuditService.LogAsync(
            user.Id,
            "RecoveryCodeLogin",
            GetIpAddress(),
            GetDevice(), geo.Country, geo.City);

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken,
            SessionId = refreshToken.Id,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }

    public async Task<ApiResponse<byte[]>> GetTwoFactorQrCodeAsync()
    {
        var setupResult = await GetTwoFactorSetupAsync();

        if (!setupResult.Success || setupResult.Data == null)
            return ApiResponse<byte[]>.Fail(setupResult.Message);

        var qrBytes = _qrCodeService.GenerateQrCodePng(
            setupResult.Data.AuthenticatorUri);

        return ApiResponse<byte[]>.Ok(qrBytes);
    }

    public async Task<ApiResponse<List<TrustedDeviceResponse>>>
    GetTrustedDevicesAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return ApiResponse<List<TrustedDeviceResponse>>
                .Fail("User is not authenticated");
        }

        var devices = await _context.TrustedDevices
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                !x.IsRevoked &&
                x.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new TrustedDeviceResponse
            {
                Id = x.Id,
                DeviceName = x.DeviceName,
                IpAddress = x.IpAddress,
                UserAgent = x.UserAgent,
                ExpiresAt = x.ExpiresAt,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return ApiResponse<List<TrustedDeviceResponse>>
            .Ok(devices);
    }

    public async Task<ApiResponse<string>>RevokeTrustedDeviceAsync(int deviceId)
    {
        var userId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<string>
                .Fail("User is not authenticated");

        var device = await _context.TrustedDevices
            .FirstOrDefaultAsync(x =>
                x.Id == deviceId &&
                x.UserId == userId);

        if (device == null)
            return ApiResponse<string>
                .Fail("Trusted device not found");

        device.IsRevoked = true;

        await _context.SaveChangesAsync();
        var geo = await GetGeoLocationAsync();
        await _securityAuditService.LogAsync(
            userId,
            "TrustedDeviceRevoked",
            GetIpAddress(),
            GetDevice(), geo.Country, geo.City);

        return ApiResponse<string>
            .Ok("Trusted device revoked successfully");
    }

    public async Task<ApiResponse<string>>
    RevokeAllTrustedDevicesAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (string.IsNullOrWhiteSpace(userId))
            return ApiResponse<string>
                .Fail("User is not authenticated");

        var devices = await _context.TrustedDevices
            .Where(x =>
                x.UserId == userId &&
                !x.IsRevoked)
            .ToListAsync();

        foreach (var device in devices)
        {
            device.IsRevoked = true;
        }

        await _context.SaveChangesAsync();
        var geo = await GetGeoLocationAsync();
        await _securityAuditService.LogAsync(
            userId,
            "AllTrustedDevicesRevoked",
            GetIpAddress(),
            GetDevice(), geo.Country, geo.City);

        return ApiResponse<string>
            .Ok("All trusted devices revoked successfully");
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
    private string GenerateQrCodeUri(string email, string unformattedKey)
    {
        const string issuer = "CarServiceBookingSystem";

        return $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(email)}" +
               $"?secret={unformattedKey}" +
               $"&issuer={Uri.EscapeDataString(issuer)}" +
               $"&digits=6";
    }

    public async Task CleanupExpiredTrustedDevicesAsync()
    {
        var expiredDevices = await _context.TrustedDevices
            .Where(x =>
                !x.IsRevoked &&
                x.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync();

        foreach (var device in expiredDevices)
        {
            device.IsRevoked = true;
        }

        await _context.SaveChangesAsync();
    }

    private async Task<bool> IsTrustedDeviceAsync(string userId,string rawToken)
    {
        var tokenHash = TokenHasher.Hash(rawToken);
        var currentFingerprintHash = GetDeviceFingerprintHash();

        return await _context.TrustedDevices.AnyAsync(x =>
            x.UserId == userId &&
            x.TokenHash == tokenHash &&
            x.DeviceFingerprintHash == currentFingerprintHash &&
            !x.IsRevoked &&
            x.ExpiresAt > DateTime.UtcNow);
    }

    private async Task<string> CreateTrustedDeviceAsync(ApplicationUser user)
    {
        var rawToken = Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));

        var trustedDevice = new TrustedDevice
        {
            UserId = user.Id,
            TokenHash = TokenHasher.Hash(rawToken),
            DeviceName = GetDevice(),
            IpAddress = GetIpAddress(),
            UserAgent = _httpContextAccessor
                .HttpContext?
                .Request
                .Headers["User-Agent"]
                .ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            IsRevoked = false,
            DeviceFingerprintHash = GetDeviceFingerprintHash()
        };

        await _context.TrustedDevices.AddAsync(trustedDevice);
        await _context.SaveChangesAsync();

        return rawToken;
    }

    private async Task<bool> IsSuspiciousLoginAsync(string userId)
    {
        var ip = GetIpAddress();
        var device = GetDevice();

        var hasPreviousLoginFromSameDevice = await _context.SecurityAuditLogs
            .AnyAsync(x =>
                x.UserId == userId &&
                x.EventType == "LoginSuccess" &&
                x.IpAddress == ip &&
                x.Device == device);

        return !hasPreviousLoginFromSameDevice;
    }

    private async Task<GeoLocationResult> GetGeoLocationAsync()
    {
        return await _geoLocationService.GetLocationAsync(GetIpAddress());
    }

    private string? GetDeviceFingerprintHash()
    {
        var fingerprint = _httpContextAccessor.HttpContext?
            .Request.Headers["X-Device-Fingerprint"]
            .ToString();

        if (string.IsNullOrWhiteSpace(fingerprint))
            return null;

        return TokenHasher.Hash(fingerprint);
    }

    private async Task RevokeRefreshTokenFamilyAsync(string tokenFamilyId,string reason)
    {
        var tokens = await _context.RefreshTokens
            .Where(x => x.TokenFamilyId == tokenFamilyId && !x.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = GetIpAddress();
            token.RevocationReason = reason;
        }

        await _context.SaveChangesAsync();
    }

    private async Task<IList<string>> GetUserPermissionsAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var permissions = new List<string>();

        foreach (var role in roles)
        {
            var roleClaims = await _roleManager.GetClaimsAsync(
                new IdentityRole(role));

            permissions.AddRange(
                roleClaims
                    .Where(x => x.Type == CustomClaimTypes.Permission)
                    .Select(x => x.Value));
        }

        return permissions.Distinct().ToList();
    }
}