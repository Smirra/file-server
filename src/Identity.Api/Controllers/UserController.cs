using System.Security.Cryptography;
using System.Text;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<UserController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IUserStore<User> _userStore;
    private readonly IUserEmailStore<User> _emailStore;
    private readonly IConfiguration _config;
    // private readonly IEmailSender _emailSender;

    public UserController(SignInManager<User> signInManager,
                          UserManager<User> userManager,
                          IUserStore<User> userStore,
                          ILogger<UserController> logger,
                          IConfiguration config)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = (IUserEmailStore<User>)_userStore;
        _logger = logger;
        _config = config;
    }

    [TempData]
    public string? ErrorMessage { get; set; }

    public class UserDto
    {
        public string UserName { get; set; } = null!;
        public string? FirstName { get; set; } = "";
        public bool? IsAdmin { get; set; } = false;
    }

    [HttpPost("signin")]
    // [SwaggerRequestExample(typeof(LoginModel), typeof(LoginModelExamples))]
    public async Task<IActionResult> SignIn([FromBody] LoginModel input, bool obfuscate)
    {
        var user = await _signInManager.UserManager.FindByEmailAsync(input.Email);

        if (user != null)
        {
            if (obfuscate)
            {
                try
                {
                    input.Password = DeObfuscate(input.Password);
                }
                catch (FormatException)
                {
                    return BadRequest("Incorrect obfuscation.");
                }
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, input.Password, input.RememberMe, lockoutOnFailure: false);
            if (result.IsLockedOut)
            {
                return Forbid("Account is locked.");
            }
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                var isAdmin = await _signInManager.UserManager.IsInRoleAsync(user, "Admin");

                var userDto = new UserDto
                {
                    UserName = user.UserName!,
                    FirstName = user.FirstName,
                    IsAdmin = isAdmin,
                };

                return Ok(new
                {
                    user = userDto,
                });

            }
        }

        return Unauthorized("Wrong email or password.");
    }

    [HttpPost("signout")]
    public new async Task<IActionResult> SignOut()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        return Ok();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel register, bool obfuscate)
    {
        var user = new User()
        {
            UserName = register.UserName,
            FirstName = register.FirstName,
            LastName = register.LastName,
        };

        await _userStore.SetUserNameAsync(user, register.UserName, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, register.Email, CancellationToken.None);

        var result = await _userManager.CreateAsync(user, register.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User created a new account with password.");

            if (obfuscate)
            {
                try
                {
                    register.Password = DeObfuscate(register.Password);
                }
                catch (FormatException)
                {
                    return BadRequest("Incorrect obfuscation.");
                }
            }

            // var userId = await _userManager.GetUserIdAsync(user);
            // var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            // var callbackUrl = Url.Page(
            //     "/Account/ConfirmEmail",
            //     pageHandler: null,
            //     values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
            //     protocol: Request.Scheme);

            // await _emailSender.SendEmailAsync(register.Email, "Confirm your email",
            //     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            // if (_userManager.Options.SignIn.RequireConfirmedAccount)
            // {
            //     return RedirectToPage("RegisterConfirmation", new { email = register.Email, returnUrl = returnUrl });
            // }
            // else
            // {
            //     await _signInManager.SignInAsync(user, isPersistent: false);
            //     return LocalRedirect(returnUrl);
            // }

            await _signInManager.SignInAsync(user, register.RememberMe);
            var isAdmin = await _signInManager.UserManager.IsInRoleAsync(user, "Admin");

            var userDto = new UserDto
            {
                UserName = user.UserName ?? "",
                FirstName = user.FirstName,
                IsAdmin = isAdmin,
            };

            return Ok(
                new
                {
                    user = userDto,
                }
            );
        }

        return BadRequest(result.Errors);
    }

    private string DeObfuscate(string obfuscatedBase64Str)
    {
        var ivStr = _config["OBFUSCATE_IV"] ?? throw new Exception("No IV provided.");
        var keyStr = _config["OBFUSCATE_KEY"] ?? throw new Exception("No key provided.");

        using Aes aesAlg = Aes.Create();
        aesAlg.IV = Encoding.UTF8.GetBytes(ivStr);
        aesAlg.Key = Encoding.UTF8.GetBytes(keyStr);

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        byte[] obfuscatedPassword = Convert.FromBase64String(obfuscatedBase64Str);

        using MemoryStream msDecrypt = new(obfuscatedPassword);
        using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
        using StreamReader srDecrypt = new(csDecrypt);
        string plaintext = srDecrypt.ReadToEnd();

        return plaintext;
    }
}