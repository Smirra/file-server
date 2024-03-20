using System.Security.Claims;
using Identity.Api.Authorization;
using Identity.Api.Configuration;
using Identity.Api.Data;
using Identity.Api.Models;
using Identity.Api.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Controllers;

[ApiController]
[Route("file")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class FilesApiOperationsController(IIdentityApiRepository identityApiRepository) : ControllerBase
{
    [HttpGet("can-create")]
    [Authorize(Constants.CanCreateFile)]
    public IActionResult CanCreate()
    {
        return Ok("User can create files.");
    }

    [HttpPut]
    [Authorize(Constants.CanCreateFile)]
    public async Task<IActionResult> CreateFiles([FromBody] IEnumerable<Guid> fileIds)
    {
        var userId = GetUserId();
        var ownerRole = await identityApiRepository.GetResourceRole(Constants.FileOwnerRole);
        var ownerRoleId = ownerRole!.Id;

        var userFileIds = await identityApiRepository.GetUserFileIds(fileIds);
        var filesToAdd = fileIds.Where(fileId => !userFileIds.Contains(fileId)).Select(fileId => new UserFile
        {
            FileId = fileId,
            UserId = userId,
            ResourceRoleId = ownerRoleId,
            CanShare = true
        });
        var filesNotAdded = fileIds.Except(userFileIds).ToList();
        if (!filesToAdd.Any())
        {
            return Ok("All Ids already exist in db.");
        }

        await identityApiRepository.AddUserFiles(filesToAdd);
        await identityApiRepository.SaveChanges();

        return CreatedAtAction(nameof(CreateFiles), new { filesToAdd, filesNotAdded });
    }

    [HttpGet]
    public async Task<IActionResult> GetFiles([FromQuery] string? ids)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole(Constants.AdminRole);
        IEnumerable<Guid> fileIds;
        var fileIdsNotFound = new List<Guid>();

        if (string.IsNullOrEmpty(ids))
        {
            if (isAdmin)
            {
                fileIds = await identityApiRepository.GetAllUserFileIds();
            }
            else
            {
                fileIds = await identityApiRepository.GetAllUserFileIds(userId);
            }

            return Ok(new {fileIds});
        }
        
        List<Guid> guids = ids.Split(',').Select(Guid.Parse).ToList();
        if (isAdmin)
        {
            fileIds = await identityApiRepository.GetUserFileIds(guids);
        }
        else
        {
            fileIds = await identityApiRepository.GetUserFileIds(userId, guids);
        }
        
        if (!fileIds.Any())
        {
            return NotFound("No files found.");
        }

        fileIdsNotFound = guids.Except(fileIds).ToList();

        return Ok(new { fileIds, fileIdsNotFound });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteFiles(string ids)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole(Constants.AdminRole);
        var guids = ids.Split(',').Select(Guid.Parse).ToList();

        IEnumerable<Guid> deletedFileIds;
        var fileIdsNotFound = new List<Guid>();

        if (isAdmin)
        {
            deletedFileIds = await identityApiRepository.DeleteUserFilesById(guids);
        }
        else
        {
            deletedFileIds = await identityApiRepository.DeleteUserFilesById(userId, guids);
        }

        if (!deletedFileIds.Any())
        {
            return NotFound("No file(s) found.");
        }

        fileIdsNotFound = guids.Except(deletedFileIds).ToList();

        await identityApiRepository.SaveChanges();

        return Ok(new { fileIds = deletedFileIds, fileIdsNotFound });
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? throw new Exception("No current user found."));
    }

}