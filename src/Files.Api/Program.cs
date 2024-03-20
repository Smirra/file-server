using Files.Api.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Files.Api.Services;
using Files.Api.Repositories;
using Files.Api.Middlewares;
using Polly;
using Shared.Data;
using Files.Api.Data;

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
services.AddHttpContextAccessor();

services.AddHealthChecks();

services.AddTransient<JwtDelegatingHandler>();

services.AddHttpClient<IIdentityHttpClientService, IdentityHttpClientService>(options =>
    {
        options.BaseAddress = new Uri(config["IDENTITY_URL"]
                                      ?? throw new Exception("IDENTITY_URL not set."));
    })
    .AddHttpMessageHandler<JwtDelegatingHandler>()
    .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(
        [
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10)
        ]));

if (env == "Development")
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
        {
            options.OperationFilter<SwaggerFileOperation>();
            options.EnableAnnotations();
        });
}

string? connectionString = config.GetConnectionString("FILES");
services.AddDbContext<FilesApiDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

string? fsKeysConnectionString = config.GetConnectionString("KEYS_FILES");
services.AddDbContext<FSKeysContext>(options =>
{
    options.UseNpgsql(fsKeysConnectionString,
                      builder => builder.MigrationsAssembly("Shared"));
});

services.AddDataProtection()
    .PersistKeysToDbContext<FSKeysContext>()
    .SetApplicationName("FileServer");

services.AddScoped<IFilesApiRepository, FilesApiRepository>();
services.AddSingleton<IFileHandlerService, FileHandlerService>();
services.AddScoped<IJwtService, JwtService>();

services.Configure<ForwardedHeadersOptions>(options =>  // Don't forget this is important as hell.
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme, options =>
    {
        options.Cookie.Name = "session";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ReturnUrlParameter = "returnUrl";
        options.LoginPath = "/signin";
    });

services.AddAuthorization();

var app = builder.Build();

app.UseForwardedHeaders(); // Very very very important for proxy. Otherwise, the app will not work behind a proxy.

app.MapHealthChecks("/healthz");

app.UseAuthentication();

// Must go after UseAuthentication and before UseAuthorization to make returnurl right. Hopefully?
app.UseRouting();
app.UsePathBase("/api/files");

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<RequestSizeLimitMiddleware>();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<FilesApiDbContext>();
var keysDbContext = scope.ServiceProvider.GetRequiredService<FSKeysContext>();

if (dbContext.Database.GetPendingMigrations().Any())
{
    dbContext.Database.Migrate();
}

if (keysDbContext.Database.GetPendingMigrations().Any())
{
    keysDbContext.Database.Migrate();
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