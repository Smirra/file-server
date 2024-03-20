using System.Globalization;
using System.Net;
using System.Net.Mime;
using Files.Api.Data;
using Files.Api.Models;
using Files.Api.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace Files.Api.Repositories;
public class FilesApiRepository(FilesApiDbContext dbContext, IHttpContextAccessor httpContextAccessor, IFileHandlerService fileHandler) : IFilesApiRepository
{
    private HttpRequest Request => httpContextAccessor.HttpContext?.Request ?? throw new NullReferenceException("HttpRequest is null.");

    public async Task<FileItem?> GetFileItem(Guid id)
    {
        return await dbContext.Files.FindAsync(id);
    }

    public async Task<IEnumerable<FileItem?>> GetFileItems(IEnumerable<Guid> fileIds)
    {
        return await dbContext.Files.Where(f => fileIds.Contains(f.Id)).ToListAsync();
    }

    public async Task<IEnumerable<FileItem?>> GetAllFileItems()
    {
        return await dbContext.Files.ToListAsync();
    }

    public async Task AddFileItem(FileItem fileItem)
    {
        await dbContext.Files.AddAsync(fileItem);
    }

    public async Task AddFileItems(IEnumerable<FileItem> fileItems)
    {
        await dbContext.Files.AddRangeAsync(fileItems);
    }

    public void DeleteFileItem(FileItem fileItem)
    {
        dbContext.Files.Remove(fileItem);
    }

    public void DeleteFileItems(IEnumerable<FileItem> fileItems)
    {
        dbContext.Files.RemoveRange(fileItems);
    }
    public void DeleteFileOnDisk(string fileName)
    {
        fileHandler.DeleteFiles(new List<string> {fileName});
    }

    public void DeleteFilesOnDisk(IEnumerable<string> fileNames)
    {
        fileHandler.DeleteFiles(fileNames);
    }

    public async Task SaveChanges()
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> CheckSumExists(string checkSum)
    {
        return await dbContext.Files.AsNoTracking()
                                     .AnyAsync(e => e.CheckSum == checkSum);
    }

    public async Task<bool> CheckSumExists(Guid id, string checkSum)
    {
        return await dbContext.Files.AsNoTracking()
                                     .AnyAsync(e => e.CheckSum == checkSum && e.Id == id);
    }

    public bool HasChanges()
    {
        return dbContext.ChangeTracker.HasChanges();
    }

    public async Task<(List<FileItem>, FormValueProvider)> ReadMultipartFormAndPersistFiles()
    {
        var fileItems = new List<FileItem>();
        var formAccumulator = new KeyValueAccumulator();
        var multipartBoundary = Request.GetMultipartBoundary();

        var reader = new MultipartReader(multipartBoundary, Request.Body);
        var section = await reader.ReadNextSectionAsync();

        while (section != null)
        {
            var fileSection = section?.AsFileSection();
            var formDataSection = section?.AsFormDataSection();

            if (fileSection != null)
            {
                var currentFileItem = new FileItem()
                {
                    Name = WebUtility.HtmlEncode(fileSection.FileName),
                    ContentType = fileSection.Section.ContentType ?? MediaTypeNames.Application.Octet,
                    RandomFileName = Path.GetRandomFileName(),
                    UploadedAt = DateTime.UtcNow,
                    LastModifiedAt = DateTime.UtcNow
                };

                _ = fileSection?.FileStream ?? throw new ArgumentNullException(nameof(fileSection.FileStream));
                (currentFileItem.CheckSum, currentFileItem.Size) = await fileHandler.WriteFileAndGetCheckSum(fileSection.FileStream,
                                                                                                              currentFileItem.RandomFileName);

                fileItems.Add(currentFileItem);
            }
            else if (formDataSection != null)
            {
                var val = WebUtility.HtmlEncode(await formDataSection.GetValueAsync());
                formAccumulator.Append(formDataSection.Name, val);
            }

            section = await reader.ReadNextSectionAsync();
        }

        var formValueProvider = new FormValueProvider(BindingSource.Form,
                                                      new FormCollection(formAccumulator.GetResults()),
                                                      CultureInfo.CurrentCulture);

        return (fileItems, formValueProvider);
    }

    public Stream GetStreamFromDisk(string fileName)
    {
        return fileHandler.GetFileStream(fileName);
    }
}