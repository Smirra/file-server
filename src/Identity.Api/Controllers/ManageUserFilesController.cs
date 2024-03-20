
using Identity.Api.Authorization;
using Identity.Api.Configuration;
using Identity.Api.Models;
using Identity.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("manage")]
[Authorize]
public class ManageUserFilesController(IAuthorizationService authorizationService,
                                 UserManager<User> userManager,
                                 IIdentityApiRepository identityApiRepository) : ControllerBase
{

    // Probably not a necessarry endpoint.
    [HttpGet("{fileId}/authorized/{operation}")]
    public async Task<IActionResult> IsAuthorized(string operation)
    {
        // In memory for now..
        var isAllowedOperation = typeof(Constants.AllowedOperations).GetProperties()
                                                                    .Any(op => op.GetValue(null) as string == operation);
        if (!isAllowedOperation)
        {
            return BadRequest("Operation does not exist in scope.");
        }

        var IsAuthorized = await authorizationService.AuthorizeAsync(User, $"FileOperation{operation}");
        if (!IsAuthorized.Succeeded)
        {
            return Forbid("User does not have permission to perform this operation.");
        }

        return Ok("User is authorized to perform this operation.");
    }

    [HttpPut("{fileId}/give-ownership/{targetUserId}")]
    public async Task<IActionResult> GiveOwnership(Guid fileId, Guid targetUserId)
    {
        var targetUserExists = identityApiRepository.UserExists(targetUserId);
        if (!targetUserExists)
        {
            return NotFound("User not found.");
        }

        var fileOwnerRole = await identityApiRepository.GetResourceRole(Constants.FileOwnerRole)
        ?? throw new Exception("FileOwner role not found.");
        var targetUserFile = await identityApiRepository.GetUserFileWithUserAndResourceRole(targetUserId, fileId);
        if (targetUserFile != null)
        {
            if (targetUserFile.ResourceRole.Name == Constants.FileOwnerRole)
            {
                return Ok("User is already an owner of this file.");
            }

            var targetUserIsAdmin = await userManager.IsInRoleAsync(targetUserFile.User, Constants.AdminRole);
            if (targetUserIsAdmin)
            {
                return Ok("Admins already have ownership of all files.");
            }
            targetUserFile.ResourceRoleId = fileOwnerRole.Id;
            targetUserFile.CanShare = true;
        }
        else
        {
            targetUserFile = new UserFile
            {
                FileId = fileId,
                UserId = targetUserId,
                ResourceRoleId = fileOwnerRole.Id,
                CanShare = true
            };

            await identityApiRepository.AddUserFile(targetUserFile);
        }

        await identityApiRepository.SaveChanges();

        return Ok(new { newOwner = new { targetUserFile.UserId, targetUserFile.FileId } });
    }

    [HttpPut("{fileId}/give-view-access/{targetUserId}")]
    [AuthorizeOperations(Constants.ShareOperation)]
    public async Task<IActionResult> GiveViewAccess(Guid fileId,
                                                    Guid targetUserId,
                                                    [FromQuery] bool allowShare = false)
    {
        var targetUserExists = identityApiRepository.UserExists(targetUserId);
        if (!targetUserExists)
        {
            return NotFound("User not found.");
        }

        // If targetUserFile exists, it means the user can already view the file, as an absolute minimum.
        var targetUserFile = await identityApiRepository.GetUserFile(targetUserId, fileId);
        if (targetUserFile != null)
        {
            return Ok("User can already atleast view this file.");
        }

        var viewerRole = await identityApiRepository.GetResourceRole(Constants.FileViewerRole)
                         ?? throw new Exception("File viewer role not found.");
        await identityApiRepository.AddUserFile(new UserFile
        {
            FileId = fileId,
            UserId = targetUserId,
            ResourceRoleId = viewerRole.Id,
            CanShare = allowShare
        });

        await identityApiRepository.SaveChanges();

        return Ok("View access given to user.");
    }

    [HttpPatch("{fileId}/update-share-rights/{targetUserId}")]
    public async Task<IActionResult> UpdateShareRights(Guid fileId, Guid targetUserId, [FromQuery] bool allowShare)
    {
        var targetUserFile = await identityApiRepository.GetUserFileWithResourceRole(targetUserId, fileId);
        if (targetUserFile?.ResourceRole.Name == Constants.FileOwnerRole)
        {
            return Ok("User is owner of this file.");
        }

        if (targetUserFile == null)
        {
            return Ok("User does not have access to this file.");
        }

        targetUserFile.CanShare = allowShare;

        await identityApiRepository.SaveChanges();

        return Ok(targetUserFile.CanShare ? "User share rights have been added." : "User share rights have been removed.");
    }

    [HttpPut("{fileId}/remove-file-access/{targetUserId}")]
    public async Task<IActionResult> RemoveFileAccess(Guid fileId, Guid targetUserId)
    {
        var targetUserFile = await identityApiRepository.GetUserFileWithResourceRole(targetUserId, fileId);
        if (targetUserFile == null)
        {
            return Ok("User does not have access to this file already.");
        }

        var targetUserRole = targetUserFile.ResourceRole.Name;

        // Only admins are allowed to remove fileowners from file, they can't remove each other once they are a fileowner.
        if (targetUserRole != Constants.FileOwnerRole || User.IsInRole(Constants.AdminRole))
        {
            identityApiRepository.DeleteUserFile(targetUserFile);
            
            await identityApiRepository.SaveChanges();
            return Ok("File removed from user.");
        }

        return Forbid("Only admins can remove fileowners from files.");
    }
}