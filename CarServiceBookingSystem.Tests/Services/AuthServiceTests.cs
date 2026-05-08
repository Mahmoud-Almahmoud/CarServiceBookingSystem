using CarServiceBookingSystem.Application.Common;
using CarServiceBookingSystem.Application.DTOs.Auth;
using CarServiceBookingSystem.Application.Interfaces;
using CarServiceBookingSystem.Domain.Entities;
using CarServiceBookingSystem.Infrastructure.Identity;
using CarServiceBookingSystem.Infrastructure.Services;
using CarServiceBookingSystem.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace CarServiceBookingSystem.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_Should_Fail_When_Email_Already_Exists()
    {
        // Arrange
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

        var tokenServiceMock = new Mock<ITokenService>();

        var authService = new AuthService(
            userManagerMock.Object,
            tokenServiceMock.Object,
            context);

        var request = new RegisterRequest
        {
            FullName = "Test User",
            Email = "test@test.com",
            PhoneNumber = "0500000000",
            Password = "Test123!"
        };

        // Act
        var result = await authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Email already exists");
    }

    [Fact]
    public async Task RegisterAsync_Should_Create_User_And_Return_Tokens()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var userManagerMock = UserManagerMockHelper.Create();

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

        var tokenServiceMock = new Mock<ITokenService>();

        tokenServiceMock
            .Setup(x => x.CreateAccessTokenAsync(It.IsAny<AuthUser>()))
            .ReturnsAsync("access-token");

        tokenServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        var authService = new AuthService(
            userManagerMock.Object,
            tokenServiceMock.Object,
            context);

        var request = new RegisterRequest
        {
            FullName = "Test User",
            Email = "test@test.com",
            PhoneNumber = "0500000000",
            Password = "Test123!"
        };

        var result = await authService.RegisterAsync(request);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("access-token");
        result.Data.RefreshToken.Should().Be("refresh-token");

        context.RefreshTokens.Count().Should().Be(1);
    }

    [Fact]
    public async Task LoginAsync_Should_Fail_When_User_Not_Found()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var userManagerMock = UserManagerMockHelper.Create();

        userManagerMock
         .Setup(x => x.FindByEmailAsync("missing@test.com"))
         .ReturnsAsync((ApplicationUser?)null);

        var tokenServiceMock = new Mock<ITokenService>();

        var authService = new AuthService(
            userManagerMock.Object,
            tokenServiceMock.Object,
            context);

        var request = new LoginRequest
        {
            Email = "missing@test.com",
            Password = "Test123!"
        };

        var result = await authService.LoginAsync(request);

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

        var users = new List<ApplicationUser> { user }.AsQueryable();

        var userManagerMock = UserManagerMockHelper.Create();

        userManagerMock
            .Setup(x => x.FindByEmailAsync("test@test.com"))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(x => x.CheckPasswordAsync(user, "Test123!"))
            .ReturnsAsync(true);

        userManagerMock
            .Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        var tokenServiceMock = new Mock<ITokenService>();

        tokenServiceMock
            .Setup(x => x.CreateAccessTokenAsync(It.IsAny<AuthUser>()))
            .ReturnsAsync("access-token");

        tokenServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        var authService = new AuthService(
            userManagerMock.Object,
            tokenServiceMock.Object,
            context);

        var request = new LoginRequest
        {
            Email = "test@test.com",
            Password = "Test123!"
        };

        var result = await authService.LoginAsync(request);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("access-token");
        result.Data.RefreshToken.Should().Be("refresh-token");

        context.RefreshTokens.Count().Should().Be(1);
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_Fail_When_Token_Is_Invalid()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var userManagerMock = UserManagerMockHelper.Create();
        var tokenServiceMock = new Mock<ITokenService>();

        var authService = new AuthService(
            userManagerMock.Object,
            tokenServiceMock.Object,
            context);

        var request = new RefreshTokenRequest
        {
            RefreshToken = "invalid-token"
        };

        var result = await authService.RefreshTokenAsync(request);

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
            Token = "old-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
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

        userManagerMock
            .Setup(x => x.FindByIdAsync("user-id"))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        var tokenServiceMock = new Mock<ITokenService>();

        tokenServiceMock
            .Setup(x => x.CreateAccessTokenAsync(It.IsAny<AuthUser>()))
            .ReturnsAsync("new-access-token");

        tokenServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("new-refresh-token");

        var authService = new AuthService(
            userManagerMock.Object,
            tokenServiceMock.Object,
            context);

        var request = new RefreshTokenRequest
        {
            RefreshToken = "old-refresh-token"
        };

        var result = await authService.RefreshTokenAsync(request);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be("new-access-token");
        result.Data.RefreshToken.Should().Be("new-refresh-token");

        context.RefreshTokens.Count().Should().Be(2);

        context.RefreshTokens
            .First(x => x.Token == "old-refresh-token")
            .IsRevoked
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task LogoutAsync_Should_Fail_When_RefreshToken_Is_Invalid()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        var userManagerMock = UserManagerMockHelper.Create();
        var tokenServiceMock = new Mock<ITokenService>();

        var authService = new AuthService(
            userManagerMock.Object,
            tokenServiceMock.Object,
            context);

        var request = new LogoutRequest
        {
            RefreshToken = "invalid-token"
        };

        var result = await authService.LogoutAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid refresh token");
    }
    [Fact]
    public async Task LogoutAsync_Should_Revoke_RefreshToken_When_Token_Is_Valid()
    {
        await using var context = TestDbContextFactory.CreateDbContext();

        context.RefreshTokens.Add(new RefreshToken
        {
            UserId = "user-id",
            Token = "valid-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false
        });

        await context.SaveChangesAsync();

        var userManagerMock = UserManagerMockHelper.Create();
        var tokenServiceMock = new Mock<ITokenService>();

        var authService = new AuthService(
            userManagerMock.Object,
            tokenServiceMock.Object,
            context);

        var request = new LogoutRequest
        {
            RefreshToken = "valid-refresh-token"
        };

        var result = await authService.LogoutAsync(request);

        result.Success.Should().BeTrue();

        var token = context.RefreshTokens
            .First(x => x.Token == "valid-refresh-token");

        token.IsRevoked.Should().BeTrue();
    }
}