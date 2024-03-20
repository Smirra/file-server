using Microsoft.AspNetCore.Mvc;
using Files.Api.Filters;
using Microsoft.AspNetCore.Authorization;
using Files.Api.Models;
using Files.Api.Repositories;
using Files.Api.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Files.Api.Controllers;

[ApiController]
[Route("/")]
[Authorize]
public class FilesController(ILogger<FilesController> logger, IFilesApiRepository filesApiRespository, IIdentityHttpClientService identityHttpClientService) : ControllerBase
{
    public class FileRspDto
    {
        public IEnumerable<Guid>? FileIds { get; set; }
        public IEnumerable<Guid>? FileIdsNotFound { get; set; }
    }

    [HttpGet("info")]
    public async Task<IActionResult> GetFilesInformation(
        [FromQuery, SwaggerParameter("id1,id2,id3,...,id***n***", Required = false)] string? ids)
    {
        var httpResponseMessage = await identityHttpClientService.GetFiles(ids);
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            return Problem($"{httpResponseMessage.Content.ReadAsStringAsync().Result}",
               statusCode: (int)httpResponseMessage.StatusCode,
               title: "Could not get file(s).");
        }
        var fileRspDto = await httpResponseMessage.Content.ReadFromJsonAsync<FileRspDto>();
        if (fileRspDto?.FileIds == null)
        {
            return NotFound("No files found.");
        }
        httpResponseMessage.Dispose();

        var fileItems = await filesApiRespository.GetFileItems(fileRspDto.FileIds);
        if (fileItems == null)
        {
            return NotFound();
        }

        return Ok(fileItems);
    }

    [HttpGet("download/{id}")]
    public async Task<IActionResult> DownloadFile(Guid id)
    {
        var httpResponseMessage = await identityHttpClientService.GetFiles(id.ToString());
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            return Problem($"You are not authorized to perform this action.");
        }
        var fileRspDto = await httpResponseMessage.Content.ReadFromJsonAsync<FileRspDto>();
        if (fileRspDto?.FileIds == null)
        {
            return NotFound();
        }
        httpResponseMessage.Dispose();


        var fileItem = await filesApiRespository.GetFileItem(fileRspDto.FileIds.First());
        if (fileItem == null)
        {
            return NotFound();
        }

        try
        {
            var fs = filesApiRespository.GetStreamFromDisk(fileItem.RandomFileName);

            return File(fs, fileItem.ContentType, fileItem.Name);
        }
        catch (FileNotFoundException)
        {
            return Problem("File not found on disk.");
        }
    }

    [HttpPut]
    [DisableFormValueModelBinding]
    [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
    public async Task<ActionResult<FileItem>> PutFile(Guid id)
    {
        var httpResponseMessage = await identityHttpClientService.PutFile(id);
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            return Problem($"You are not authorized to upload this file.");
        }
        httpResponseMessage.Dispose();


        var currentTime = DateTime.UtcNow;
        var fileItem = await filesApiRespository.GetFileItem(id);
        var fileItemExistsInDb = true;
        if (fileItem == null)
        {
            fileItem = new FileItem()
            {
                Id = id,
                UploadedAt = currentTime,
                LastModifiedAt = currentTime
            };
            fileItemExistsInDb = false;
        }

        (var newFileItems, var formValueProvider) = await filesApiRespository.ReadMultipartFormAndPersistFiles();
        if (newFileItems.Count > 1)
        {
            return BadRequest("Only one file is allowed at a time.");
        }
        if (newFileItems.Count != 0)
        {
            var newFileItem = newFileItems.First();

            if (await filesApiRespository.CheckSumExists(id, newFileItem.CheckSum))
            {
                filesApiRespository.DeleteFileOnDisk(newFileItem.RandomFileName);
            }
            else
            {
                fileItem.Name = newFileItem.Name;
                fileItem.ContentType = newFileItem.ContentType;
                fileItem.Size = newFileItem.Size;
                fileItem.CheckSum = newFileItem.CheckSum;
                if (fileItemExistsInDb)
                {
                    filesApiRespository.DeleteFileOnDisk(fileItem.RandomFileName);

                    fileItem.LastModifiedAt = currentTime;
                    fileItem.RandomFileName = newFileItem.RandomFileName;
                }
                else
                {
                    fileItem.RandomFileName = newFileItem.RandomFileName;

                    await filesApiRespository.AddFileItem(fileItem);
                }
            }
        }
        else
        {
            if (!fileItemExistsInDb)
            {
                return BadRequest("Could not find any file to update metadata on.");
            }
        }

        var bindingSuccessful = await TryUpdateModelAsync(fileItem,
                                          prefix: "",
                                          valueProvider: formValueProvider);

        if (!filesApiRespository.HasChanges())
        {
            return Ok("No changes were made.");
        }
        if (bindingSuccessful)
        {
            fileItem.LastModifiedAt = currentTime;
        }

        await filesApiRespository.SaveChanges();

        return fileItem;
    }

    [HttpPost]
    [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [DisableFormValueModelBinding]
    public async Task<ActionResult<IEnumerable<FileItem>>> PostFiles()
    {
        var canCreateRsp = await identityHttpClientService.CanCreate();
        if (!canCreateRsp.IsSuccessStatusCode)
        {
            return Problem($"You are not authorized to upload files.");
        }
        canCreateRsp.Dispose();


        (var fileItems, _) = await filesApiRespository.ReadMultipartFormAndPersistFiles();

        await filesApiRespository.AddFileItems(fileItems);
        await filesApiRespository.SaveChanges();

        var generatedIds = fileItems.Select(f => f.Id).ToList();

        var httpResponseMessage = await identityHttpClientService.PutFiles(generatedIds);

        return CreatedAtAction(nameof(PostFiles), nameof(FilesController), fileItems);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteFiles(
        [FromQuery, SwaggerParameter("id1,id2,id3,...,id***n***", Required = true)] string ids)
    {
        var httpResponseMessage = await identityHttpClientService.DeleteFiles(ids);
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            return Problem($"{httpResponseMessage.Content.ReadAsStringAsync().Result}",
                           statusCode: (int)httpResponseMessage.StatusCode,
                           title: "Could not delete file(s).");
        }

        var fileRspDto = await httpResponseMessage.Content.ReadFromJsonAsync<FileRspDto>();
        if (fileRspDto?.FileIds == null)
        {
            return NotFound("No files found.");
        }

        var fileItems = await filesApiRespository.GetFileItems(fileRspDto.FileIds);
        if (fileItems == null)
        {
            logger.LogError($"Files {fileRspDto.FileIds} are already deleted from Files API database. Services are out of sync.");
        }
        else
        {
            var randomFileNames = fileItems.Select(f => f?.RandomFileName).ToList();
            filesApiRespository.DeleteFilesOnDisk(randomFileNames!);
            filesApiRespository.DeleteFileItems(fileItems!);
            
            await filesApiRespository.SaveChanges();
        }

        return Ok(new {deleted = fileRspDto.FileIds, notFound = fileRspDto.FileIdsNotFound});
    }
}