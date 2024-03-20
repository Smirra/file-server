using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Models;
public class User : IdentityUser<Guid>
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    [JsonIgnore]
    public List<UserFile> UserFiles { get; } = [];
}