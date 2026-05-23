using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;

namespace CarServiceBookingSystem.API.Auth;

public class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var hasPermission = context.User.Claims.Any(x =>
            x.Type == CustomClaimTypes.Permission &&
            x.Value == requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}