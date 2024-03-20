using Bogus;
using Identity.Api.Configuration;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Data;
public static class SeedDb
{
    public static void Seed(this IdentityApiDbContext dbContext, IConfiguration config)
    {
        var passwordHasher = new PasswordHasher<User>();

        var fileOwnerRole = new ResourceRole(Constants.FileOwnerRole)
        {
            Id = Guid.NewGuid(),
            CanRead = true,
            CanUpdate = true,
            CanDelete = true,
        };

        var fileEditorRole = new ResourceRole(Constants.FileEditorRole)
        {
            Id = Guid.NewGuid(),
            CanRead = true,
            CanUpdate = true,
            CanDelete = false,
        };

        var fileViewerRole = new ResourceRole(Constants.FileViewerRole)
        {
            Id = Guid.NewGuid(),
            CanRead = true,
            CanUpdate = false,
            CanDelete = false,
        };
        List<ResourceRole> resourceRoles = [fileOwnerRole, fileEditorRole, fileViewerRole];

        var adminRole = new Role(Constants.AdminRole)
        {
            Id = Guid.NewGuid()
        };
        adminRole.NormalizedName = adminRole.Name!.ToUpper();

        var userRole = new Role(Constants.UserRole)
        {
            Id = Guid.NewGuid()
        };
        userRole.NormalizedName = userRole.Name!.ToUpper();

        var viewerRole = new Role(Constants.ViewerRole)
        {
            Id = Guid.NewGuid()
        };
        viewerRole.NormalizedName = viewerRole.Name!.ToUpper();
        List<Role> roles = [adminRole, userRole, viewerRole];

        if (!dbContext.Roles.Any())
        {
            dbContext.Roles.AddRange(roles);
        }

        if (!dbContext.ResourceRoles.Any())
        {
            dbContext.ResourceRoles.AddRange(resourceRoles);
        }

        if (config["ASPNETCORE_ENVIRONMENT"] == "Production")
        {
            User superuser = new()
            {
                UserName = "superuser",
                Email = "su@fs",
                EmailConfirmed = true,
                Id = Guid.NewGuid(),
                SecurityStamp = Guid.NewGuid().ToString()
            };
            superuser.NormalizedEmail = superuser.Email.ToUpper();
            superuser.NormalizedUserName = superuser.UserName.ToUpper();
            superuser.PasswordHash = passwordHasher.HashPassword(superuser, config["SUPERUSER_PW"]
                                                                            ?? throw new Exception("SUPERUSER password is not set"));

            User testuser = new()
            {
                UserName = "testuser",
                Email = "testuser@fs",
                EmailConfirmed = true,
                Id = Guid.NewGuid(),
                SecurityStamp = Guid.NewGuid().ToString()
            };
            testuser.NormalizedEmail = testuser.Email.ToUpper();
            testuser.NormalizedUserName = testuser.UserName.ToUpper();
            testuser.PasswordHash = passwordHasher.HashPassword(testuser, config["TESTUSER_PW"]
                                                                          ?? throw new Exception("TESTUSER password is not set"));
            dbContext.Users.AddRange(superuser, testuser);

            dbContext.UserRoles.AddRange(
                new IdentityUserRole<Guid>() { RoleId = adminRole.Id, UserId = superuser.Id },
                new IdentityUserRole<Guid>() { RoleId = userRole.Id, UserId = testuser.Id }
            );
        }
        else
        {
            User bob = new()
            {
                UserName = ExampleUsers.Bob.UserName,
                FirstName = "Bobby",
                LastName = "Bobson",
                Email = ExampleUsers.Bob.Email,
                EmailConfirmed = true,
                Id = new Guid("985e78eb-63a4-452e-9b93-83c57ad064d8"),
                SecurityStamp = Guid.NewGuid().ToString()
            };
            bob.NormalizedEmail = bob.Email.ToUpper();
            bob.NormalizedUserName = bob.UserName.ToUpper();
            bob.PasswordHash = passwordHasher.HashPassword(bob, ExampleUsers.Bob.Password);

            User alice = new()
            {
                UserName = ExampleUsers.Alice.UserName,
                FirstName = "Alice",
                LastName = "in Wonderland",
                Email = ExampleUsers.Alice.Email,
                EmailConfirmed = true,
                Id = new Guid("33a97690-bf83-4c8f-9cc8-ca01ffbfcc73"),
                SecurityStamp = Guid.NewGuid().ToString()
            };
            alice.NormalizedEmail = alice.Email.ToUpper();
            alice.NormalizedUserName = alice.UserName.ToUpper();
            alice.PasswordHash = passwordHasher.HashPassword(alice, ExampleUsers.Alice.Password);

            User carlos = new()
            {
                UserName = ExampleUsers.Carlos.UserName,
                FirstName = "Carlos",
                LastName = "Carlinhos",
                Email = ExampleUsers.Carlos.Email,
                EmailConfirmed = true,
                Id = new Guid("afef9d1c-865d-4cbd-a3de-6371d1f74276"),
                SecurityStamp = Guid.NewGuid().ToString()
            };
            carlos.NormalizedEmail = carlos.Email.ToUpper();
            carlos.NormalizedUserName = carlos.UserName.ToUpper();
            carlos.PasswordHash = passwordHasher.HashPassword(carlos, ExampleUsers.Carlos.Password);

            List<User> testUsers = [bob, alice, carlos];

            var userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => Guid.NewGuid())
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.EmailConfirmed, f => f.Random.Bool())
                .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email!.ToUpper())
                .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName!.ToUpper())
                .RuleFor(u => u.PasswordHash, (f, u) => passwordHasher.HashPassword(u, f.Internet.Password()));

            if (!dbContext.Users.Any())
            {
                dbContext.Users.AddRange(testUsers);
                dbContext.UserRoles.AddRange(
                    new IdentityUserRole<Guid>() { RoleId = adminRole.Id, UserId = bob.Id },
                    new IdentityUserRole<Guid>() { RoleId = userRole.Id, UserId = alice.Id },
                    new IdentityUserRole<Guid>() { RoleId = viewerRole.Id, UserId = carlos.Id }
                );

                var fakeUsers = userFaker.Generate(147);
                dbContext.Users.AddRange(fakeUsers);

                var fakeUserRoles = new List<IdentityUserRole<Guid>>();
                foreach (var user in fakeUsers)
                {
                    fakeUserRoles.Add(new IdentityUserRole<Guid>() { RoleId = userRole.Id, UserId = user.Id });
                }
                dbContext.UserRoles.AddRange(fakeUserRoles);
            }
        }

        dbContext.SaveChanges();
    }
}