using CarServiceBookingSystem.API.Auth;
using CarServiceBookingSystem.Application.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CarServiceBookingSystem.UnitTests.Auth;

public class PermissionPolicyProviderTests
{
    [Fact]
    public async Task GetPolicyAsync_Should_Create_PermissionPolicy_When_PolicyName_Is_Permission()
    {
        var options = Options.Create(new AuthorizationOptions());

        var provider = new PermissionPolicyProvider(options);

        var policy = await provider.GetPolicyAsync(Permissions.Services.Create);

        policy.Should().NotBeNull();
        policy!.Requirements
            .OfType<PermissionRequirement>()
            .Should()
            .ContainSingle(x => x.Permission == Permissions.Services.Create);
    }

    [Fact]
    public async Task GetPolicyAsync_Should_Return_Null_When_PolicyName_Is_Not_Permission()
    {
        var options = Options.Create(new AuthorizationOptions());

        var provider = new PermissionPolicyProvider(options);

        var policy = await provider.GetPolicyAsync("AdminOnly");

        policy.Should().BeNull();
    }
}