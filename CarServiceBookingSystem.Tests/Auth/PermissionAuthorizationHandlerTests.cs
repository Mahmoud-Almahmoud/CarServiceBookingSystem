using CarServiceBookingSystem.API.Auth;
using CarServiceBookingSystem.Application.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CarServiceBookingSystem.UnitTests.Auth;

public class PermissionAuthorizationHandlerTests
{
    [Fact]
    public async Task HandleRequirementAsync_Should_Succeed_When_User_Has_Permission()
    {
        var handler = new PermissionAuthorizationHandler();

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim(CustomClaimTypes.Permission, Permissions.Services.Create)
            ], "TestAuth"));

        var requirement = new PermissionRequirement(Permissions.Services.Create);

        var context = new AuthorizationHandlerContext(
            [requirement],
            user,
            null);

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_Should_Fail_When_User_Does_Not_Have_Permission()
    {
        var handler = new PermissionAuthorizationHandler();

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim(CustomClaimTypes.Permission, Permissions.Services.View)
            ], "TestAuth"));

        var requirement = new PermissionRequirement(Permissions.Services.Create);

        var context = new AuthorizationHandlerContext(
            [requirement],
            user,
            null);

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }
}