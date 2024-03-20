using System.ComponentModel.DataAnnotations;
using Identity.Api.Data;
using Swashbuckle.AspNetCore.Filters;

namespace Identity.Api.Models;
public class LoginModel
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; } = null!;

    public bool RememberMe { get; set; }
}

public class LoginModelExamples : IMultipleExamplesProvider<LoginModel>
{
    public IEnumerable<SwaggerExample<LoginModel>> GetExamples()
    {
        yield return SwaggerExample.Create("Bob", new LoginModel
        {
            Email = ExampleUsers.Bob.Email,
            Password = ExampleUsers.Bob.Password,
            RememberMe = false,

        });
        yield return SwaggerExample.Create("Alice", new LoginModel
        {
            Email = ExampleUsers.Alice.Email,
            Password = ExampleUsers.Alice.Password,
            RememberMe = false,
        });
        yield return SwaggerExample.Create("Carlos", new LoginModel
        {
            Email = ExampleUsers.Carlos.Email,
            Password = ExampleUsers.Carlos.Password,
            RememberMe = false,
        });
    }
}