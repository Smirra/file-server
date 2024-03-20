using Files.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Files.Api.Data;
public class FilesApiDbContext(DbContextOptions<FilesApiDbContext> options) : DbContext(options)
{
    public DbSet<FileItem> Files { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");
        modelBuilder.Entity<FileItem>().Property(f => f.Id).HasDefaultValueSql("uuid_generate_v4()");
    }
}