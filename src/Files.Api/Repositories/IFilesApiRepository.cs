using Files.Api.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Files.Api.Repositories;
public interface IFilesApiRepository
{
    Task<FileItem?> GetFileItem(Guid id);
    Task<IEnumerable<FileItem?>> GetFileItems(IEnumerable<Guid> fileIds);
    Task<IEnumerable<FileItem?>> GetAllFileItems();
    Task AddFileItem(FileItem fileItem);
    Task AddFileItems(IEnumerable<FileItem> fileItems);
    void DeleteFileItem(FileItem fileItem);
    void DeleteFileItems(IEnumerable<FileItem> fileItems);
    void DeleteFileOnDisk(string fileName);
    void DeleteFilesOnDisk(IEnumerable<string> fileNames);
    Task<bool> CheckSumExists(string checkSum);
    Task<bool> CheckSumExists(Guid id, string checkSum);
    bool HasChanges();
    Task<(List<FileItem>, FormValueProvider)> ReadMultipartFormAndPersistFiles();
    Stream GetStreamFromDisk(string fileName);
    Task SaveChanges();
}