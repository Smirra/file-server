using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.DataProtection;
using Identity.Api.Data;
using Identity.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Identity.Api.Authorization;
using Identity.Api.Configuration;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Identity.Api.Repositories;
using Shared.Data;
using Microsoft.AspNetCore.Identity;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (env == "Development")
{
    DotNetEnv.Env.Load("../../.env");
    DotNetEnv.Env.Load("../../.env.dev");
}

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

config.AddEnvironmentVariables(prefix: "FILESERVER_");

services.AddControllers();
services.AddHealthChecks();

services.AddHttpContextAccessor();

if (env == "Development")
{
    services.AddSwaggerGen(options =>
    {
        options.ExampleFilters();
    });
    services.AddSwaggerExamplesFromAssemblyOf(typeof(LoginModelExamples));
}

string? connectionString = config.GetConnectionString("IDENTITY");
services.AddIdentity<User, Role>()
    .AddRoles<Role>()
    .AddEntityFrameworkStores<IdentityApiDbContext>();
services.AddDbContext<IdentityApiDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

string? fsKeysConnectionString = config.GetConnectionString("KEYS_IDENTITY");
services.AddDbContext<FSKeysContext>(options =>
{
    options.UseNpgsql(fsKeysConnectionString,
                      builder => builder.MigrationsAssembly("Shared"));
});

services.AddDataProtection()
    .PersistKeysToDbContext<FSKeysContext>()
    .SetApplicationName("FileServer");

services.AddScoped<IIdentityApiRepository, IdentityApiRepository>();

services.Configure<ForwardedHeadersOptions>(options =>  // Don't forget this is important as hell.
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.LoginPath = "/signin";
});

services.AddAuthentication()
    .AddCookie()
    .AddJwtBearer(options =>
        {
            string? publicKeyPath = config["JWT_PUBLICKEYPATH"];
            var publicKey = File.ReadAllText(publicKeyPath
                                             ?? throw new IOException("No public key path provided."));
            var rsaPublicKey = RSA.Create();
            rsaPublicKey.ImportFromPem(publicKey);
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = config["JWT_AUDIENCE"],
                ValidIssuer = config["JWT_ISSUER"],
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(rsaPublicKey),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    if (context.AuthenticateFailure is SecurityTokenExpiredException)
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized - Token Expired.");
                    }
                    else
                    {
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsync("Forbidden.");
                    }
                }
            };
        });

services.AddSingleton<IAuthorizationPolicyProvider, AuthorizeOperationsPolicyProvider>();
services.AddScoped<IAuthorizationHandler, RepositoryAuthzHandler>();
services.AddSingleton<IAuthorizationHandler, AdminAuthorizationHandler>();

services.AddAuthorizationBuilder()
    .AddPolicy(Constants.CanCreateFile, policy =>
        policy.RequireRole(Constants.AdminRole, Constants.UserRole))
    .AddPolicy(Constants.IsFileOwner, policy =>
    {
        policy.Requirements.Add(new OwnerRequirement());
    });

var app = builder.Build();
var logger = app.Logger;

app.UseForwardedHeaders();

app.MapHealthChecks("/healthz");

app.UseAuthentication();

// Must go after UseAuthentication and before UseAuthorization to make returnurl right. Hopefully not breaking something else?
app.UsePathBase("/api/identity");
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<IdentityApiDbContext>();
var keysDbContext = scope.ServiceProvider.GetRequiredService<FSKeysContext>();

if (dbContext.Database.GetPendingMigrations().Any())
{
    dbContext.Database.Migrate();
}
if (keysDbContext.Database.GetPendingMigrations().Any())
{
    keysDbContext.Database.Migrate();
}
if (!await dbContext.Users.AnyAsync())
{
    dbContext.Seed(config);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
    app.UseExceptionHandler("/Error");
}

app.Run();


public partial class Program { }