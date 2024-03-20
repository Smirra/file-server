using Identity.Api.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity.Api.Authorization;

internal class AuthorizeOperationsPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    const string POLICY_PREFIX = Constants.FileOperation;

    private DefaultAuthorizationPolicyProvider BackupPolicyProvider { get; } = new DefaultAuthorizationPolicyProvider(options);

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var operation = policyName[POLICY_PREFIX.Length..];
        if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
        {
            var policy = new AuthorizationPolicyBuilder(IdentityConstants.ApplicationScheme);

            policy.AddRequirements(new OperationsRequirement(operation));

            return Task.FromResult<AuthorizationPolicy?>(policy.Build());
        }

        return BackupPolicyProvider.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => BackupPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => BackupPolicyProvider.GetFallbackPolicyAsync()!;
}

internal class AuthorizeOperationsAttribute : AuthorizeAttribute
{
    const string POLICY_PREFIX = Constants.FileOperation;

    public AuthorizeOperationsAttribute(string? Operation = null) => Policy = $"{POLICY_PREFIX}{Operation}";
}