using Microsoft.AspNetCore.Identity;


namespace Identity.Api.Models;
public class Role : IdentityRole<Guid>
{
    public Role() : base() { }

    public Role(string roleName) : base(roleName) { }

}