using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Auth;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Authentication;
using CarServiceBookingSystem.Infrastructure.Identity;
using CarServiceBookingSystem.UnitTests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;

namespace CarServiceBookingSystem.UnitTests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_Should_Fail_When_Email_Already_Exists()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var userManagerMock = UserManagerMockHelper.Create();

        userManagerMock
            .Setup(x => x.FindByEmailAsync("test@test.com"))
            .ReturnsAsync(new ApplicationUser
            {
                Id = "user-id",
                Email = "test@test.com",
                FullName = "Test User"
            });

        var authService = AuthServiceTestFactory.Create(context, userManagerMock);

        var result = await authService.RegisterAsync(new RegisterRequest
        {
            FullName = "Test User",
            Email = "test@test.com",
            PhoneNumber = "0500000000",
            Password = "Test123!"
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Email already exists");
    }

    [Fact]
    public async Task RegisterAsync_Should_Create_User_And_Return_Tokens()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var userManagerMock = UserManagerMockHelper.Create();
        var tokenServiceMock = new Mock<ITokenService>();
        var backgroundJobServiceMock = new Mock<IBackgroundJobService>();

        userManagerMock
            .Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("email-confirmation-token");

        userManagerMock
            .Setup(x => x.FindByEmailAsync("test@test.com"))
            .ReturnsAsync((ApplicationUser?)null);

        userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), "Test123!"))
            .ReturnsAsync(IdentityResult.Success);

        userManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
            .ReturnsAsync(IdentityResult.Success);

        userManagerMock
            .Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { "User" });

        tokenServiceMock
            .Setup(x => x.CreateAccessTokenAsync(It.IsAny<AuthUser>()))
            .ReturnsAsync("access-token");

        tokenServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock,
            tokenServiceMock: tokenServiceMock,
            backgroundJobServiceMock: backgroundJobServiceMock);

        var result = await authService.RegisterAsync(new RegisterRequest
        {
            FullName = "Test User",
            Email = "test@test.com",
            PhoneNumber = "0500000000",
            Password = "Test123!"
        });

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("access-token");
        result.Data.RefreshToken.Should().Be("refresh-token");

        context.RefreshTokens.Count().Should().Be(1);

        backgroundJobServiceMock.Verify(x =>
            x.EnqueueEmail(
                "test@test.com",
                "Confirm your email",
                It.Is<string>(body => body.Contains("email-confirmation-token"))),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_Should_Fail_When_User_Not_Found()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var userManagerMock = UserManagerMockHelper.Create();

        userManagerMock
            .Setup(x => x.FindByEmailAsync("missing@test.com"))
            .ReturnsAsync((ApplicationUser?)null);

        var authService = AuthServiceTestFactory.Create(context, userManagerMock);

        var result = await authService.LoginAsync(new LoginRequest
        {
            Email = "missing@test.com",
            Password = "Test123!"
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Tokens_When_Credentials_Are_Valid()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "test@test.com",
            UserName = "test@test.com",
            FullName = "Test User"
        };

        var userManagerMock = UserManagerMockHelper.Create();
        var tokenServiceMock = new Mock<ITokenService>();

        userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com")).ReturnsAsync(user);
        userManagerMock.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        userManagerMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        userManagerMock.Setup(x => x.CheckPasswordAsync(user, "Test123!")).ReturnsAsync(true);
        userManagerMock.Setup(x => x.GetTwoFactorEnabledAsync(user)).ReturnsAsync(false);
        userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
        userManagerMock.Setup(x => x.ResetAccessFailedCountAsync(user)).ReturnsAsync(IdentityResult.Success);

        tokenServiceMock.Setup(x => x.CreateAccessTokenAsync(It.IsAny<AuthUser>())).ReturnsAsync("access-token");
        tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock,
            tokenServiceMock: tokenServiceMock);

        var result = await authService.LoginAsync(new LoginRequest
        {
            Email = "test@test.com",
            Password = "Test123!"
        });

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("access-token");
        result.Data.RefreshToken.Should().Be("refresh-token");

        context.RefreshTokens.Count().Should().Be(1);
        userManagerMock.Verify(x => x.ResetAccessFailedCountAsync(user), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_Fail_When_Token_Is_Invalid()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var userManagerMock = UserManagerMockHelper.Create();
        var authService = AuthServiceTestFactory.Create(context, userManagerMock);

        var result = await authService.RefreshTokenAsync(new RefreshTokenRequest
        {
            RefreshToken = "invalid-token"
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid or expired refresh token");
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_Return_New_Tokens_When_Token_Is_Valid()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        context.RefreshTokens.Add(new RefreshToken
        {
            UserId = "user-id",
            Token = TokenHasher.Hash("old-refresh-token"),
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "test@test.com",
            FullName = "Test User"
        };

        var userManagerMock = UserManagerMockHelper.Create();
        var tokenServiceMock = new Mock<ITokenService>();

        userManagerMock.Setup(x => x.FindByIdAsync("user-id")).ReturnsAsync(user);
        userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

        tokenServiceMock.Setup(x => x.CreateAccessTokenAsync(It.IsAny<AuthUser>())).ReturnsAsync("new-access-token");
        tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("new-refresh-token");

        var httpContextAccessor = CreateHttpContextAccessor("user-id", "1");

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock,
            tokenServiceMock: tokenServiceMock,
            httpContextAccessor: httpContextAccessor);

        var result = await authService.RefreshTokenAsync(new RefreshTokenRequest
        {
            RefreshToken = "old-refresh-token"
        });

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("new-access-token");
        result.Data.RefreshToken.Should().Be("new-refresh-token");

        context.RefreshTokens.Count().Should().Be(2);

        context.RefreshTokens
            .First(x => x.Token == TokenHasher.Hash("old-refresh-token"))
            .IsRevoked
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task LogoutAsync_Should_Fail_When_RefreshToken_Is_Invalid()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var userManagerMock = UserManagerMockHelper.Create();

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock,
            httpContextAccessor: CreateHttpContextAccessor("user-id", "1"));

        var result = await authService.LogoutAsync(new LogoutRequest
        {
            RefreshToken = "invalid-token"
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Active session not found");
    }

    [Fact]
    public async Task LogoutAsync_Should_Revoke_RefreshToken_When_Token_Is_Valid()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var refreshToken = new RefreshToken
        {
            UserId = "user-id",
            Token = TokenHasher.Hash("valid-refresh-token"),
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();

        var userManagerMock = UserManagerMockHelper.Create();

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock,
            httpContextAccessor: CreateHttpContextAccessor("user-id", refreshToken.Id.ToString()));

        var result = await authService.LogoutAsync(new LogoutRequest
        {
            RefreshToken = "valid-refresh-token"
        });

        result.Success.Should().BeTrue();

        var token = context.RefreshTokens.First(x => x.Token == TokenHasher.Hash("valid-refresh-token"));
        token.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_Revoke_All_Tokens_When_Revoked_Token_Is_Reused()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        context.RefreshTokens.AddRange(
            new RefreshToken
            {
                UserId = "user-id",
                Token = TokenHasher.Hash("reused-token"),
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = true,
                CreatedAt = DateTime.UtcNow,
                RevokedAt = DateTime.UtcNow
            },
            new RefreshToken
            {
                UserId = "user-id",
                Token = TokenHasher.Hash("active-token"),
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            });

        await context.SaveChangesAsync();

        var userManagerMock = UserManagerMockHelper.Create();
        var authService = AuthServiceTestFactory.Create(context, userManagerMock);

        var result = await authService.RefreshTokenAsync(new RefreshTokenRequest
        {
            RefreshToken = "reused-token"
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Refresh token reuse detected. All sessions have been revoked.");

        context.RefreshTokens
            .Where(x => x.UserId == "user-id")
            .All(x => x.IsRevoked)
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task GetActiveSessionsAsync_Should_Return_Only_Active_User_Sessions()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        context.RefreshTokens.AddRange(
            new RefreshToken
            {
                UserId = "user-id",
                Token = TokenHasher.Hash("active-token"),
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = "127.0.0.1",
                Device = "Chrome"
            },
            new RefreshToken
            {
                UserId = "user-id",
                Token = TokenHasher.Hash("revoked-token"),
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = true,
                CreatedAt = DateTime.UtcNow,
                Device = "Firefox"
            },
            new RefreshToken
            {
                UserId = "other-user-id",
                Token = TokenHasher.Hash("other-token"),
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow,
                Device = "Safari"
            });

        await context.SaveChangesAsync();

        var userManagerMock = UserManagerMockHelper.Create();

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock,
            httpContextAccessor: CreateHttpContextAccessor("user-id"));

        var result = await authService.GetActiveSessionsAsync();

        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data![0].Device.Should().Be("Chrome");
    }

    [Fact]
    public async Task RevokeSessionAsync_Should_Revoke_Selected_Session()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var refreshToken = new RefreshToken
        {
            UserId = "user-id",
            Token = TokenHasher.Hash("session-token"),
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();

        var userManagerMock = UserManagerMockHelper.Create();

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock,
            httpContextAccessor: CreateHttpContextAccessor("user-id", "999"));

        var result = await authService.RevokeSessionAsync(refreshToken.Id);

        result.Success.Should().BeTrue();

        var token = await context.RefreshTokens.FindAsync(refreshToken.Id);

        token!.IsRevoked.Should().BeTrue();
        token.RevocationReason.Should().Be("Session revoked by user");
    }

    [Fact]
    public async Task RevokeSessionAsync_Should_Fail_When_Revoking_Current_Session()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var refreshToken = new RefreshToken
        {
            UserId = "user-id",
            Token = TokenHasher.Hash("current-session-token"),
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();

        var userManagerMock = UserManagerMockHelper.Create();

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock,
            httpContextAccessor: CreateHttpContextAccessor("user-id", refreshToken.Id.ToString()));

        var result = await authService.RevokeSessionAsync(refreshToken.Id);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("You cannot revoke the current session. Use logout instead.");
    }

    [Fact]
    public async Task LogoutAllDevicesAsync_Should_Revoke_All_Active_User_Tokens()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        context.RefreshTokens.AddRange(
            new RefreshToken
            {
                UserId = "user-id",
                Token = TokenHasher.Hash("token-1"),
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            },
            new RefreshToken
            {
                UserId = "user-id",
                Token = TokenHasher.Hash("token-2"),
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            },
            new RefreshToken
            {
                UserId = "other-user-id",
                Token = TokenHasher.Hash("other-token"),
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            });

        await context.SaveChangesAsync();

        var userManagerMock = UserManagerMockHelper.Create();

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock,
            httpContextAccessor: CreateHttpContextAccessor("user-id"));

        var result = await authService.LogoutAllDevicesAsync();

        result.Success.Should().BeTrue();

        context.RefreshTokens
            .Where(x => x.UserId == "user-id")
            .All(x => x.IsRevoked)
            .Should()
            .BeTrue();

        context.RefreshTokens
            .Where(x => x.UserId == "other-user-id")
            .All(x => !x.IsRevoked)
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task LoginAsync_Should_Fail_When_User_Is_Locked_Out()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "locked@test.com",
            UserName = "locked@test.com",
            FullName = "Locked User"
        };

        var userManagerMock = UserManagerMockHelper.Create();

        userManagerMock.Setup(x => x.FindByEmailAsync("locked@test.com")).ReturnsAsync(user);
        userManagerMock.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        userManagerMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(true);

        var authService = AuthServiceTestFactory.Create(context, userManagerMock);

        var result = await authService.LoginAsync(new LoginRequest
        {
            Email = "locked@test.com",
            Password = "Wrong123!"
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Account is temporarily locked. Try again later.");
    }

    [Fact]
    public async Task LoginAsync_Should_Call_AccessFailed_When_Password_Is_Invalid()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "test@test.com",
            UserName = "test@test.com",
            FullName = "Test User"
        };

        var userManagerMock = UserManagerMockHelper.Create();

        userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com")).ReturnsAsync(user);
        userManagerMock.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        userManagerMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        userManagerMock.Setup(x => x.CheckPasswordAsync(user, "Wrong123!")).ReturnsAsync(false);
        userManagerMock.Setup(x => x.AccessFailedAsync(user)).ReturnsAsync(IdentityResult.Success);

        var authService = AuthServiceTestFactory.Create(context, userManagerMock);

        var result = await authService.LoginAsync(new LoginRequest
        {
            Email = "test@test.com",
            Password = "Wrong123!"
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid credentials");

        userManagerMock.Verify(x => x.AccessFailedAsync(user), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmailAsync_Should_Fail_When_User_Not_Found()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var userManagerMock = UserManagerMockHelper.Create();

        userManagerMock
            .Setup(x => x.FindByIdAsync("missing-user-id"))
            .ReturnsAsync((ApplicationUser?)null);

        var authService = AuthServiceTestFactory.Create(context, userManagerMock);

        var result = await authService.ConfirmEmailAsync("missing-user-id", "token");

        result.Success.Should().BeFalse();
        result.Message.Should().Be("User not found");
    }

    [Fact]
    public async Task ConfirmEmailAsync_Should_Return_Success_When_Token_Is_Valid()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "test@test.com",
            FullName = "Test User"
        };

        var userManagerMock = UserManagerMockHelper.Create();

        userManagerMock.Setup(x => x.FindByIdAsync("user-id")).ReturnsAsync(user);
        userManagerMock.Setup(x => x.ConfirmEmailAsync(user, "valid-token")).ReturnsAsync(IdentityResult.Success);

        var authService = AuthServiceTestFactory.Create(context, userManagerMock);

        var result = await authService.ConfirmEmailAsync("user-id", "valid-token");

        result.Success.Should().BeTrue();
        result.Data.Should().Be("Email confirmed successfully");
    }

    [Fact]
    public async Task ForgotPasswordAsync_Should_Return_Generic_Message_When_User_Not_Found()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var userManagerMock = UserManagerMockHelper.Create();

        userManagerMock
            .Setup(x => x.FindByEmailAsync("missing@test.com"))
            .ReturnsAsync((ApplicationUser?)null);

        var authService = AuthServiceTestFactory.Create(context, userManagerMock);

        var result = await authService.ForgotPasswordAsync(new ForgotPasswordRequest
        {
            Email = "missing@test.com"
        });

        result.Success.Should().BeTrue();
        result.Data.Should().Be("If the email exists, a reset password link has been sent.");
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Fail_When_User_Not_Found()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var userManagerMock = UserManagerMockHelper.Create();

        userManagerMock
            .Setup(x => x.FindByEmailAsync("missing@test.com"))
            .ReturnsAsync((ApplicationUser?)null);

        var authService = AuthServiceTestFactory.Create(context, userManagerMock);

        var result = await authService.ResetPasswordAsync(new ResetPasswordRequest
        {
            Email = "missing@test.com",
            Token = "token",
            NewPassword = "NewPass123!"
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid reset request");
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Return_Success_When_Token_Is_Valid()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "test@test.com",
            FullName = "Test User"
        };

        var userManagerMock = UserManagerMockHelper.Create();

        userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com")).ReturnsAsync(user);
        userManagerMock.Setup(x => x.ResetPasswordAsync(user, "valid-token", "NewPass123!")).ReturnsAsync(IdentityResult.Success);

        var authService = AuthServiceTestFactory.Create(context, userManagerMock);

        var result = await authService.ResetPasswordAsync(new ResetPasswordRequest
        {
            Email = "test@test.com",
            Token = "valid-token",
            NewPassword = "NewPass123!"
        });

        result.Success.Should().BeTrue();
        result.Data.Should().Be("Password reset successfully");
    }

    [Fact]
    public async Task ChangePasswordAsync_Should_Return_Success_When_Password_Changed()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "test@test.com",
            FullName = "Test User"
        };

        var userManagerMock = UserManagerMockHelper.Create();
        var backgroundJobServiceMock = new Mock<IBackgroundJobService>();

        userManagerMock.Setup(x => x.FindByIdAsync("user-id")).ReturnsAsync(user);
        userManagerMock.Setup(x => x.ChangePasswordAsync(user, "OldPass123!", "NewPass123!")).ReturnsAsync(IdentityResult.Success);

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock,
            httpContextAccessor: CreateHttpContextAccessor("user-id"),
            backgroundJobServiceMock: backgroundJobServiceMock);

        var result = await authService.ChangePasswordAsync(new ChangePasswordRequest
        {
            CurrentPassword = "OldPass123!",
            NewPassword = "NewPass123!"
        });

        result.Success.Should().BeTrue();
        result.Data.Should().Be("Password changed successfully");

        backgroundJobServiceMock.Verify(x =>
            x.EnqueueEmail(
                "test@test.com",
                "Password Changed",
                It.Is<string>(body => body.Contains("password was changed"))),
            Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_Should_Fail_When_Current_Password_Is_Invalid()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "test@test.com",
            FullName = "Test User"
        };

        var userManagerMock = UserManagerMockHelper.Create();

        userManagerMock.Setup(x => x.FindByIdAsync("user-id")).ReturnsAsync(user);
        userManagerMock
            .Setup(x => x.ChangePasswordAsync(user, "WrongPass123!", "NewPass123!"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError
            {
                Description = "Incorrect password."
            }));

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock,
            httpContextAccessor: CreateHttpContextAccessor("user-id"));

        var result = await authService.ChangePasswordAsync(new ChangePasswordRequest
        {
            CurrentPassword = "WrongPass123!",
            NewPassword = "NewPass123!"
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Password change failed");
        result.Errors.Should().Contain("Incorrect password.");
    }

    [Fact]
    public async Task ChangePasswordAsync_Should_Revoke_Other_Sessions_And_TrustedDevices()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var currentToken = new RefreshToken
        {
            UserId = "user-id",
            Token = TokenHasher.Hash("current-token"),
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        var otherToken = new RefreshToken
        {
            UserId = "user-id",
            Token = TokenHasher.Hash("other-token"),
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        context.RefreshTokens.AddRange(currentToken, otherToken);

        context.TrustedDevices.Add(new TrustedDevice
        {
            UserId = "user-id",
            TokenHash = TokenHasher.Hash("trusted-device-token"),
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            IsRevoked = false
        });

        await context.SaveChangesAsync();

        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "test@test.com",
            FullName = "Test User"
        };

        var userManagerMock = UserManagerMockHelper.Create();

        userManagerMock.Setup(x => x.FindByIdAsync("user-id")).ReturnsAsync(user);
        userManagerMock.Setup(x => x.ChangePasswordAsync(user, "OldPass123!", "NewPass123!")).ReturnsAsync(IdentityResult.Success);

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock,
            httpContextAccessor: CreateHttpContextAccessor("user-id", currentToken.Id.ToString()));

        var result = await authService.ChangePasswordAsync(new ChangePasswordRequest
        {
            CurrentPassword = "OldPass123!",
            NewPassword = "NewPass123!"
        });

        result.Success.Should().BeTrue();

        var currentSession = await context.RefreshTokens.FindAsync(currentToken.Id);
        currentSession!.IsRevoked.Should().BeFalse();

        var otherSession = await context.RefreshTokens.FindAsync(otherToken.Id);
        otherSession!.IsRevoked.Should().BeTrue();

        context.TrustedDevices
            .Where(x => x.UserId == "user-id")
            .All(x => x.IsRevoked)
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task LoginAsync_Should_Log_SuspiciousGeoLogin_When_Login_From_New_Location()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        context.SecurityAuditLogs.Add(new SecurityAuditLog
        {
            UserId = "user-id",
            EventType = "LoginSuccess",
            IpAddress = "1.1.1.1",
            Device = "Chrome",
            Country = "United Arab Emirates",
            City = "Abu Dhabi",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        });

        await context.SaveChangesAsync();

        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "test@test.com",
            UserName = "test@test.com",
            FullName = "Test User"
        };

        var userManagerMock = UserManagerMockHelper.Create();
        var tokenServiceMock = new Mock<ITokenService>();
        var securityAuditServiceMock = new Mock<ISecurityAuditService>();
        var geoLocationServiceMock = new Mock<IGeoLocationService>();

        userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com")).ReturnsAsync(user);
        userManagerMock.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        userManagerMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        userManagerMock.Setup(x => x.CheckPasswordAsync(user, "Test123!")).ReturnsAsync(true);
        userManagerMock.Setup(x => x.GetTwoFactorEnabledAsync(user)).ReturnsAsync(false);
        userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
        userManagerMock.Setup(x => x.ResetAccessFailedCountAsync(user)).ReturnsAsync(IdentityResult.Success);

        tokenServiceMock.Setup(x => x.CreateAccessTokenAsync(It.IsAny<AuthUser>())).ReturnsAsync("access-token");
        tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");

        geoLocationServiceMock
            .Setup(x => x.GetLocationAsync(It.IsAny<string?>()))
            .ReturnsAsync(new GeoLocationResult
            {
                Country = "Germany",
                City = "Berlin"
            });

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock,
            tokenServiceMock: tokenServiceMock,
            securityAuditServiceMock: securityAuditServiceMock,
            geoLocationServiceMock: geoLocationServiceMock);

        var result = await authService.LoginAsync(new LoginRequest
        {
            Email = "test@test.com",
            Password = "Test123!"
        });

        result.Success.Should().BeTrue();

        securityAuditServiceMock.Verify(x =>
            x.LogAsync(
                "user-id",
                "SuspiciousLoginDetected",
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                "Germany",
                "Berlin",
                It.Is<string?>(details => details!.Contains("Login from a new IP address or device"))),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_Should_Not_Log_SuspiciousGeoLogin_When_Login_From_Known_Location()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        context.SecurityAuditLogs.Add(new SecurityAuditLog
        {
            UserId = "user-id",
            EventType = "LoginSuccess",
            Country = "Germany",
            City = "Berlin",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        });

        await context.SaveChangesAsync();

        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "test@test.com",
            UserName = "test@test.com",
            FullName = "Test User"
        };

        var userManagerMock = UserManagerMockHelper.Create();
        var tokenServiceMock = new Mock<ITokenService>();
        var securityAuditServiceMock = new Mock<ISecurityAuditService>();
        var geoLocationServiceMock = new Mock<IGeoLocationService>();

        userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com")).ReturnsAsync(user);
        userManagerMock.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        userManagerMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        userManagerMock.Setup(x => x.CheckPasswordAsync(user, "Test123!")).ReturnsAsync(true);
        userManagerMock.Setup(x => x.GetTwoFactorEnabledAsync(user)).ReturnsAsync(false);
        userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
        userManagerMock.Setup(x => x.ResetAccessFailedCountAsync(user)).ReturnsAsync(IdentityResult.Success);

        tokenServiceMock.Setup(x => x.CreateAccessTokenAsync(It.IsAny<AuthUser>())).ReturnsAsync("access-token");
        tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");

        geoLocationServiceMock
            .Setup(x => x.GetLocationAsync(It.IsAny<string?>()))
            .ReturnsAsync(new GeoLocationResult
            {
                Country = "Germany",
                City = "Berlin"
            });

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock,
            tokenServiceMock: tokenServiceMock,
            securityAuditServiceMock: securityAuditServiceMock,
            geoLocationServiceMock: geoLocationServiceMock);

        var result = await authService.LoginAsync(new LoginRequest
        {
            Email = "test@test.com",
            Password = "Test123!"
        });

        result.Success.Should().BeTrue();

        securityAuditServiceMock.Verify(x =>
            x.LogAsync(
                "user-id",
                "SuspiciousGeoLoginDetected",
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_Fail_When_DeviceFingerprint_Does_Not_Match()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        context.RefreshTokens.Add(new RefreshToken
        {
            UserId = "user-id",
            Token = TokenHasher.Hash("valid-refresh-token"),
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            DeviceFingerprintHash = TokenHasher.Hash("original-device")
        });

        await context.SaveChangesAsync();

        var userManagerMock = UserManagerMockHelper.Create();

        var httpContextAccessor = CreateHttpContextAccessor("user-id");
        httpContextAccessor.HttpContext!.Request.Headers["X-Device-Fingerprint"] = "different-device";

        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "test@test.com",
            FullName = "Test User"
        };

        userManagerMock
            .Setup(x => x.FindByIdAsync("user-id"))
            .ReturnsAsync(user);

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock,
            httpContextAccessor: httpContextAccessor);

        var result = await authService.RefreshTokenAsync(new RefreshTokenRequest
        {
            RefreshToken = "valid-refresh-token"
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Refresh token is no longer valid from this device.");

        var token = context.RefreshTokens.First();

        token.IsRevoked.Should().BeTrue();
        token.RevocationReason.Should().Be("Device fingerprint mismatch");
    }

    [Fact]
    public async Task LoginAsync_Should_Require_TwoFactor_When_TrustedDeviceFingerprint_Does_Not_Match()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        context.TrustedDevices.Add(new TrustedDevice
        {
            UserId = "user-id",
            TokenHash = TokenHasher.Hash("trusted-device-token"),
            DeviceFingerprintHash = TokenHasher.Hash("original-device"),
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            IsRevoked = false
        });

        await context.SaveChangesAsync();

        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = "test@test.com",
            UserName = "test@test.com",
            FullName = "Test User"
        };

        var userManagerMock = UserManagerMockHelper.Create();

        userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com")).ReturnsAsync(user);
        userManagerMock.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        userManagerMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        userManagerMock.Setup(x => x.CheckPasswordAsync(user, "Test123!")).ReturnsAsync(true);
        userManagerMock.Setup(x => x.GetTwoFactorEnabledAsync(user)).ReturnsAsync(true);
        userManagerMock.Setup(x => x.ResetAccessFailedCountAsync(user)).ReturnsAsync(IdentityResult.Success);

        var httpContextAccessor = CreateHttpContextAccessor("user-id");
        httpContextAccessor.HttpContext!.Request.Headers["X-Device-Fingerprint"] = "different-device";

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock,
            httpContextAccessor: httpContextAccessor);

        var result = await authService.LoginAsync(new LoginRequest
        {
            Email = "test@test.com",
            Password = "Test123!",
            TrustedDeviceToken = "trusted-device-token"
        });

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.RequiresTwoFactor.Should().BeTrue();
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_Revoke_Only_Compromised_Token_Family_When_Revoked_Token_Is_Reused()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        context.RefreshTokens.AddRange(
            new RefreshToken
            {
                UserId = "user-id",
                Token = TokenHasher.Hash("reused-token"),
                TokenFamilyId = "family-1",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = true,
                CreatedAt = DateTime.UtcNow,
                RevokedAt = DateTime.UtcNow
            },
            new RefreshToken
            {
                UserId = "user-id",
                Token = TokenHasher.Hash("active-token-same-family"),
                TokenFamilyId = "family-1",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            },
            new RefreshToken
            {
                UserId = "user-id",
                Token = TokenHasher.Hash("active-token-other-family"),
                TokenFamilyId = "family-2",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            });

        await context.SaveChangesAsync();

        var userManagerMock = UserManagerMockHelper.Create();

        var authService = AuthServiceTestFactory.Create(
            context,
            userManagerMock);

        var result = await authService.RefreshTokenAsync(new RefreshTokenRequest
        {
            RefreshToken = "reused-token"
        });

        result.Success.Should().BeFalse();

        result.Message.Should().Be("Refresh token reuse detected. All sessions have been revoked.");

        context.RefreshTokens
            .Where(x => x.TokenFamilyId == "family-1")
            .All(x => x.IsRevoked)
            .Should()
            .BeTrue();

        context.RefreshTokens
            .Where(x => x.TokenFamilyId == "family-2")
            .All(x => !x.IsRevoked)
            .Should()
            .BeTrue();
    }

    private static HttpContextAccessor CreateHttpContextAccessor(
        string userId,
        string? sessionId = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId)
        };

        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            claims.Add(new Claim("session_id", sessionId));
        }

        return new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "TestAuth"))
            }
        };
    }
}