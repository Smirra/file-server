using Identity.Api.Models;

namespace Identity.Api.Repositories;

public interface IIdentityApiRepository
{
    bool UserExists(Guid id);
    Task<User?> GetUser(Guid id);
    Task<PagedList<User>> GetUsers(string? query, string? sortOrder, int pageIndex, int pageSize);
    Task<ResourceRole?> GetResourceRole(string roleName);
    Task<UserFile?> GetUserFile(Guid userId, Guid fileId);
    Task<UserFile?> GetUserFileWithResourceRole(Guid userId, Guid fileId);
    Task<UserFile?> GetUserFileWithUserAndResourceRole(Guid userId, Guid fileId);
    Task<IEnumerable<UserFile>> GetUserFiles(IEnumerable<Guid> fileIds);
    Task<IEnumerable<UserFile>> GetUserFiles(Guid userId, IEnumerable<Guid> fileIds);

    Task AddUserFile(UserFile userFile);
    Task AddUserFiles(IEnumerable<UserFile> userFiles);
    Task<IEnumerable<Guid>> GetUserFileIds(IEnumerable<Guid> fileIds);
    Task<IEnumerable<Guid>> GetUserFileIds(Guid userId, IEnumerable<Guid> fileIds);
    Task<IEnumerable<Guid>> GetUserFileIdsToDelete(Guid userId, IEnumerable<Guid> fileIds);
    Task<IEnumerable<Guid>> GetAllUserFileIds();
    Task<IEnumerable<Guid>> GetAllUserFileIds(Guid userId);
    void DeleteUserFile(UserFile userFile);
    Task<IEnumerable<Guid>> DeleteUserFilesById(IEnumerable<Guid> fileIds);
    Task<IEnumerable<Guid>> DeleteUserFilesById(Guid userId, IEnumerable<Guid> fileIds);

    Task SaveChanges();
}