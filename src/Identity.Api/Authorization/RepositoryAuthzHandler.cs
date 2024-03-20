using System.Security.Claims;
using Identity.Api.Configuration;
using Identity.Api.Models;
using Identity.Api.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Authorization;

public class RepositoryAuthzHandler(IIdentityApiRepository identityApiRepository,
                                           IHttpContextAccessor httpContextAccessor) : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var pendingRequirements = context.PendingRequirements.ToList();
        var httpContext = httpContextAccessor.HttpContext;

        if (context.User == null || httpContext == null)
        {
            return;
        }

        var userId = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var fileId = httpContext.GetRouteValue("fileId")?.ToString();
        if (userId == null || fileId == null)
        {
            return;
        }

        var userFile = await identityApiRepository.GetUserFileWithResourceRole(Guid.Parse(userId),
                                                                      Guid.Parse(fileId));
        if (userFile == null)
        {
            return;
        }

        foreach (var requirement in pendingRequirements)
        {
            if (requirement is OwnerRequirement ownerRequirement)
            {
                var handle = HandleOwnerRequirement(userFile);

                if (handle)
                {
                    context.Succeed(requirement);
                    return;
                }
            }
            if (requirement is OperationsRequirement operationsRequirement)
            {
                var handle = HandleOperationsRequirement(operationsRequirement, userFile);
                if (handle)
                {
                    context.Succeed(requirement);
                    return;
                }
            }
        }
    }

    private static bool HandleOperationsRequirement(OperationsRequirement requirement, UserFile userFile)
    {
        bool hasRequiredPrivilege = requirement.Operation switch
        {
            Constants.ReadOperation => userFile.ResourceRole.CanRead,
            Constants.UpdateOperation => userFile.ResourceRole.CanUpdate,
            Constants.ShareOperation => userFile.CanShare,
            _ => false,
        };

        if (hasRequiredPrivilege)
        {
            return true;
        }

        return false;
    }

    private static bool HandleOwnerRequirement(UserFile userFile)
    {
        if (userFile.ResourceRole.Name == Constants.FileOwnerRole)
        {
            return true;
        }

        return false;
    }
}