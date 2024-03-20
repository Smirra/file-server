using Identity.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Data;
public class IdentityApiDbContext(DbContextOptions<IdentityApiDbContext> options) : IdentityDbContext<User, Role, Guid>(options)
{
    public DbSet<UserFile> UserFiles { get; set; } = null!;
    public DbSet<ResourceRole> ResourceRoles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("uuid-ossp");
        modelBuilder.Entity<User>().Property(u => u.Id).HasDefaultValueSql("uuid_generate_v4()");
        modelBuilder.Entity<Role>().Property(ur => ur.Id).HasDefaultValueSql("uuid_generate_v4()");
        modelBuilder.Entity<ResourceRole>().Property(ur => ur.Id).HasDefaultValueSql("uuid_generate_v4()");

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName!.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName[6..]);
            }
        }
    }
}