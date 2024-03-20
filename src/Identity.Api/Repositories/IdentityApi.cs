using Identity.Api.Data;
using Identity.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Repositories;
public class IdentityApiRepository(IdentityApiDbContext dbContext) : IIdentityApiRepository
{
    public bool UserExists(Guid id)
    {
        return dbContext.Users.Any(u => u.Id == id);
    }

    public async Task<User?> GetUser(Guid id)
    {
        return await dbContext.Users.FindAsync(id);
    }

    public async Task<PagedList<User>> GetUsers(string? query, string? sortOrder, int pageIndex, int pageSize)
    {
        var users = dbContext.Users.AsQueryable();

        if (!string.IsNullOrEmpty(query))
        {
            pageIndex = 1;
            users =  users.Where(u => u.UserName!.Contains(query) || u.Email!.Contains(query));
        }

        if (!string.IsNullOrEmpty(sortOrder))
        {
            users = sortOrder switch
            {
                "username_desc" => users.OrderByDescending(u => u.NormalizedUserName),
                "email" => users.OrderBy(u => u.NormalizedEmail),
                "email_desc" => users.OrderByDescending(u => u.NormalizedEmail),
                _ => users.OrderBy(u => u.NormalizedUserName),
            };
        }
        else
        {
            users = users.OrderBy(u => u.NormalizedUserName);
        }

        return await PagedList<User>.CreateAsync(users, pageIndex, pageSize);
    }

    public async Task<ResourceRole?> GetResourceRole(string roleName)
    {
        return await dbContext.ResourceRoles.AsNoTracking().FirstOrDefaultAsync(ur => ur.Name == roleName);
    }

    public async Task<UserFile?> GetUserFile(Guid userId, Guid fileId)
    {
        return await dbContext.UserFiles.FirstOrDefaultAsync(uf => uf.FileId == fileId && uf.UserId == userId);
    }
    public async Task<IEnumerable<UserFile>> GetUserFiles(IEnumerable<Guid> fileIds)
    {
        return await dbContext.UserFiles.Where(uf => fileIds.Contains(uf.FileId)).ToListAsync();
    }

    public async Task<IEnumerable<UserFile>> GetUserFiles(Guid userId, IEnumerable<Guid> fileIds)
    {
        return await dbContext.UserFiles.Where(uf => fileIds.Contains(uf.FileId) && uf.UserId == userId).ToListAsync();
    }

    public async Task<UserFile?> GetUserFileWithResourceRole(Guid userId, Guid fileId)
    {
        return await dbContext.UserFiles.Include(uf => uf.ResourceRole)
                                         .FirstOrDefaultAsync(uf => uf.FileId == fileId
                                                                    && uf.UserId == userId);
    }

    public async Task<UserFile?> GetUserFileWithUserAndResourceRole(Guid userId, Guid fileId)
    {
        return await dbContext.UserFiles.Include(uf => uf.User)
                                         .Include(uf => uf.ResourceRole)
                                         .FirstOrDefaultAsync(uf => uf.FileId == fileId
                                                                    && uf.UserId == userId);
    }

    public async Task<IEnumerable<Guid>> GetUserFileIds(IEnumerable<Guid> fileIds)
    {
        return await dbContext.UserFiles.Where(uf => fileIds.Contains(uf.FileId)).Select(uf => uf.FileId).ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetUserFileIds(Guid userId, IEnumerable<Guid> fileIds)
    {
        return await dbContext.UserFiles.Where(uf => fileIds.Contains(uf.FileId) && uf.UserId == userId).Select(uf => uf.FileId).ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetUserFileIdsToDelete(Guid userId, IEnumerable<Guid> fileIds)
    {
        return await dbContext.UserFiles.Include(uf => uf.ResourceRole)
                                                .Where(uf => fileIds.Contains(uf.FileId)
                                                             && uf.UserId == userId
                                                             && uf.ResourceRole.CanDelete)
                                                .Select(uf => uf.FileId).ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetAllUserFileIds()
    {
        return await dbContext.UserFiles.Select(uf => uf.FileId).ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetAllUserFileIds(Guid userId)
    {
        return await dbContext.UserFiles.Where(uf => uf.UserId == userId).Select(uf => uf.FileId).ToListAsync();
    }

    public async Task AddUserFile(UserFile userFile)
    {
        await dbContext.UserFiles.AddAsync(userFile);
    }

    public async Task AddUserFiles(IEnumerable<UserFile> userFiles)
    {
        await dbContext.UserFiles.AddRangeAsync(userFiles);
    }

    public void DeleteUserFile(UserFile userFile)
    {
        dbContext.UserFiles.Remove(userFile);
    }

    public async Task<IEnumerable<Guid>> DeleteUserFilesById(IEnumerable<Guid> fileIds)
    {
        var userFilesToDelete = await GetUserFiles(fileIds);

        dbContext.UserFiles.RemoveRange(userFilesToDelete);

        return userFilesToDelete.Select(uf => uf.FileId);
    }

    public async Task<IEnumerable<Guid>> DeleteUserFilesById(Guid userId, IEnumerable<Guid> fileIds)
    {
        var userFilesToDelete = await dbContext.UserFiles.Include(uf => uf.ResourceRole)
                                                .Where(uf => fileIds.Contains(uf.FileId)
                                                             && uf.UserId == userId
                                                             && uf.ResourceRole.CanDelete).ToListAsync();

        dbContext.UserFiles.RemoveRange(userFilesToDelete);

        return userFilesToDelete.Select(uf => uf.FileId);
    }


    public async Task SaveChanges()
    {
        await dbContext.SaveChangesAsync();
    }
}