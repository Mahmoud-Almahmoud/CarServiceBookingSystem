using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Auth;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Application.Interfaces.IAuth;
using CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs;
using CarServiceBookingSystem.Application.Interfaces.IUtils;
using CarServiceBookingSystem.Application.Options;
using CarServiceBookingSystem.Application.Security;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Domain.Enums;
using CarServiceBookingSystem.Infrastructure.Identity;
using CarServiceBookingSystem.Infrastructure.Persistence;
using CarServiceBookingSystem.Infrastructure.Utils.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CarServiceBookingSystem.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly ISecurityAuditService _securityAuditService;
    private readonly IQrCodeService _qrCodeService;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JwtOptions _jwtOption;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        ApplicationDbContext context,IHttpContextAccessor httpContextAccessor,
        IBackgroundJobService backgroundJobService,
        ISecurityAuditService securityAuditService,
        IQrCodeService qrCodeService,
        IGeoLocationService geoLocationService, RoleManager<IdentityRole> roleManager,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _qrCodeService = qrCodeService;
        _backgroundJobService = backgroundJobService;
        _securityAuditService = securityAuditService;
        _roleManager = roleManager;
        _jwtOption = jwtOptions.Value;
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return ApiResponse<AuthResponse>.Fail("Email already exists");
        }

        return ApiResponse<AuthResponse>.Fail("Registration is currently disabled");

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

        await _userManager.AddToRoleAsync(user, Domain.Enums.Roles.User);

        await _securityAuditService.LogAsync(
           user.Id,
           SecurityAuditEventType.UserRoleAdded,
           "role added to user during registration");

        var roles = await _userManager.GetRolesAsync(user);

        var refreshToken = await CreateRefreshTokenAsync(user.Id);

        await _securityAuditService.LogAsync(user.Id,
            SecurityAuditEventType.Registration,
            "User registered successfully");

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

        await _securityAuditService.LogAsync(user.Id,
            SecurityAuditEventType.EmailConfirmationResent,
            "Confirmation email sent after registration");

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
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
            await _securityAuditService.LogAsync(
                user.Id,
                SecurityAuditEventType.EmailConfirmationFailed,
                "Email confirmation failed");

            return ApiResponse<string>.Fail(
                "Email confirmation failed",
                result.Errors.Select(x => x.Description).ToList());
        }

        await _securityAuditService.LogAsync(
            user.Id, SecurityAuditEventType.EmailConfirmed, 
            "User confirmed email successfully");

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

        
        await _securityAuditService.LogAsync(
            user.Id, 
            SecurityAuditEventType.EmailConfirmationResent, 
            "Confirmation email resent");

        return ApiResponse<string>.Ok("Confirmation email sent successfully");
    }
    public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return ApiResponse<string>.Ok("If the email exists, a reset password link has been sent.");
        }
        if (user.UserName.Equals("admin@mahmoudev.com") || user.Email.Equals("user@mahmoudev.com"))
        {
            return ApiResponse<string>.Fail("This is a special account. Please contact support for password reset.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var resetLink =
            $"https://my-frontend-domain.com/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(token)}";

        _backgroundJobService.EnqueueEmail(user.Email!,"Reset your password",$"Reset your password using this link: {resetLink}");

        
        await _securityAuditService.LogAsync(
            user.Id, SecurityAuditEventType.PasswordResetRequested, 
            "Password reset requested");

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

        
        await _securityAuditService.LogAsync(
            user.Id, 
            SecurityAuditEventType.PasswordReset,
            "Password reset successfully");

        var trustedDevices = await _context.TrustedDevices.Where(x => x.UserId == user.Id && !x.IsRevoked).ToListAsync();
        foreach (var device in trustedDevices)
        {
            device.IsRevoked = true;
        }
        await _context.SaveChangesAsync();

        await _securityAuditService.LogAsync(
            user.Id, 
            SecurityAuditEventType.TrustedDevicesRevokedAll, 
            "Trusted Devices Revoked All After Password Reset");

        await RevokeAllUserRefreshTokensAsync(user.Id, "Password Reset");

        await _securityAuditService.LogAsync(
            user.Id,
            SecurityAuditEventType.RefreshTokenFamilyRevoked,
            "Refresh Tokens Revoked All After Password Reset");

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

        if (user.UserName.Equals("admin@mahmoudev.com") || user.Email.Equals("user@mahmoudev.com"))
        {
            return ApiResponse<string>.Fail("This is a special account. Please contact support for password change.");
        }

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
        
        await _securityAuditService.LogAsync(user.Id,
            SecurityAuditEventType.TrustedDevicesRevokedAll,
            "Trusted Devices Revoked All After Password Change");

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
        await _securityAuditService.LogAsync(user.Id, 
            SecurityAuditEventType.PasswordChanged,
            "Password changed successfully");
        return ApiResponse<string>.Ok("Password changed successfully");
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
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
            await _securityAuditService.LogAsync(user.Id, 
                SecurityAuditEventType.LoginFailed,
                "Invalid password");
            return ApiResponse<AuthResponse>.Fail("Invalid credentials");
        }
        await _userManager.ResetAccessFailedCountAsync(user);

        var isSuspicious = await IsSuspiciousLoginAsync(user.Id);

        if (isSuspicious)
        {
            await _securityAuditService.LogAsync(
                user.Id,
                SecurityAuditEventType.SuspiciousLogin,
                "Login from a new IP address or device");

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

        var refreshToken = await CreateRefreshTokenAsync(user.Id);

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
        await _securityAuditService.LogAsync(user.Id,
            SecurityAuditEventType.LoginSucceeded,null);


        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
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
            await RevokeRefreshTokenFamilyAsync(storedToken.TokenFamilyId,"Refresh token reuse detected");
            await _securityAuditService.LogAsync(
                storedToken.UserId, 
                SecurityAuditEventType.RefreshTokenFamilyRevoked,
                "Refresh token reuse detected");
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
                SecurityAuditEventType.RefreshTokenRevoked,
                $"Refresh token with id ({storedToken.Id}) used from a different device fingerprint");

            return ApiResponse<AuthResponse>.Fail(
                "Refresh token is no longer valid from this device.");
        }
        
        var roles = await _userManager.GetRolesAsync(user);

        var newRefreshToken = await CreateRefreshTokenAsync(user.Id, storedToken.TokenFamilyId);

        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.ReplacedByToken = newRefreshToken.Token;
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

        await _securityAuditService.LogAsync(
               storedToken.UserId,
               SecurityAuditEventType.RefreshTokenUsed,
               $"Refresh token with id ({storedToken.Id}) used and replaced");

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
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
        await _securityAuditService.LogAsync(
            storedToken.UserId,
            SecurityAuditEventType.Logout,
            $"User logged out from session with id ({storedToken.Id})");
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

        
        await _securityAuditService.LogAsync(
           userId,
           SecurityAuditEventType.LogoutAllSessions,
           $"User logged out from all sessions");

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

        
        await _securityAuditService.LogAsync(
           userId,
           SecurityAuditEventType.SessionRevoked,
           $"User revoked session with id ({token.Id})");

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

        
        await _securityAuditService.LogAsync(
           userId,
           SecurityAuditEventType.TwoFactorEnabled,
           $"User enabled two-factor authentication");

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
        {
            await _securityAuditService.LogAsync(
               user.Id,
               SecurityAuditEventType.TwoFactorLoginFailed,
               $"User failed to log in with two-factor authentication");

            return ApiResponse<AuthResponse>.Fail("Invalid verification code");
        }

        string? trustedDeviceToken = null;

        if (request.RememberDevice)
        {
            trustedDeviceToken = await CreateTrustedDeviceAsync(user);
        }

        if (request.RememberDevice)
        {
            trustedDeviceToken = await CreateTrustedDeviceAsync(user);
        }

        var refreshToken = await CreateRefreshTokenAsync(user.Id);

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

        await _securityAuditService.LogAsync(
            user.Id,
            SecurityAuditEventType.TwoFactorLoginSucceeded,
            null);

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
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
        {
            await _securityAuditService.LogAsync(
               user.Id,
               SecurityAuditEventType.TwoFactorLoginFailed,
               $"User failed to verify two-factor code");

            return ApiResponse<string>.Fail("Invalid verification code");
        }

        await _userManager.SetTwoFactorEnabledAsync(user, false);
        
        await _securityAuditService.LogAsync(
            user.Id,
            SecurityAuditEventType.TwoFactorDisabled,
            null);

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

        
        await _securityAuditService.LogAsync(
            user.Id,
            SecurityAuditEventType.TwoFactorRecoveryCodesGenerated,
            "user generated new two-factor recovery codes");

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
            await _securityAuditService.LogAsync(
               user.Id,
               SecurityAuditEventType.TwoFactorLoginFailed,
               $"User failed to log in with recovery code");

            return ApiResponse<AuthResponse>
                .Fail("Invalid recovery code");
        }

        var refreshToken = await CreateRefreshTokenAsync(user.Id);

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
        
        await _securityAuditService.LogAsync(
            user.Id,
            SecurityAuditEventType.TwoFactorLoginSucceeded,
            null);

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
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

    public async Task<ApiResponse<List<TrustedDeviceResponse>>>GetTrustedDevicesAsync()
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

        
        await _securityAuditService.LogAsync(
            userId,
            SecurityAuditEventType.TrustedDeviceRevoked, 
            $"user revoked trusted device with ID: {device.Id}");

        return ApiResponse<string>
            .Ok("Trusted device revoked successfully");
    }

    public async Task<ApiResponse<string>>RevokeAllTrustedDevicesAsync()
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

        
        await _securityAuditService.LogAsync(
            userId,
            SecurityAuditEventType.TrustedDevicesRevokedAll,
            "user revoked all trusted devices");

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
                x.EventType == SecurityAuditEventType.LoginSucceeded &&
                x.IpAddress == ip &&
                x.Device == device);

        return !hasPreviousLoginFromSameDevice;
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

    private async Task RevokeAllUserRefreshTokensAsync(string userId, string reason)
    {
        var tokens = await _context.RefreshTokens
            .Where(x => x.UserId == userId && !x.IsRevoked)
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
        var roles = await _userManager.GetRolesAsync(user)
            ?? new List<string>();

        var permissions = new List<string>();

        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
                continue;

            var roleClaims = await _roleManager.GetClaimsAsync(role);

            permissions.AddRange(
                roleClaims
                    .Where(x => x.Type == CustomClaimTypes.Permission)
                    .Select(x => x.Value));
        }

        return permissions.Distinct().ToList();
    }

    private async Task<RefreshTokenResponse> CreateRefreshTokenAsync(string userId, string? tokenFamilyId = null)
    {
        var refreshTokenRaw = _tokenService.GenerateRefreshToken();
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = TokenHasher.Hash(refreshTokenRaw),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOption.RefreshTokenExpirationDays),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = GetIpAddress(),
            Device = GetDevice(),
            DeviceFingerprintHash = GetDeviceFingerprintHash(),
            TokenFamilyId = tokenFamilyId ?? Guid.NewGuid().ToString()
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        return new RefreshTokenResponse
        {
            Id = refreshToken.Id,
            Token = refreshTokenRaw
        };
    }

    private sealed class RefreshTokenResponse
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;  
    }
}