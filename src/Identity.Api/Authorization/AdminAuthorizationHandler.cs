using Identity.Api.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Authorization;

// Think this approach will always create an OR situation...
public class AdminAuthorizationHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var pendingRequirements = context.PendingRequirements.ToList();

        foreach (var requirement in pendingRequirements)
        {
            if (requirement is OperationsRequirement || requirement is OwnerRequirement)
            {
                if (context.User == null)
                {
                    return Task.CompletedTask;
                }

                if (context.User.IsInRole(Constants.AdminRole))
                {
                    context.Succeed(requirement);
                }
            }
        }

        return Task.CompletedTask;
    }
}
