using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CarServiceBookingSystem.API.Auth;

public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(
        IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override Task<AuthorizationPolicy?> GetPolicyAsync(
        string policyName)
    {
        if (policyName.Contains("."))
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return base.GetPolicyAsync(policyName);
    }
}