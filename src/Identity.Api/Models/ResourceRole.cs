namespace Identity.Api.Models;
public class ResourceRole
{
    public ResourceRole()
    {
    }

    public ResourceRole(string roleName)
    {
        Name = roleName;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool CanRead { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
}