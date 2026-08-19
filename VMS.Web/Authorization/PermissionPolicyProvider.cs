using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace VMS.Web.Authorization;

public class PermissionPolicyProvider
    : DefaultAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(
        IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(
        string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);

        if (policy != null)
            return policy;

        var permissionPolicy =
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(
                    new PermissionRequirement(policyName))
                .Build();

        return permissionPolicy;
    }
}