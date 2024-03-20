using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Authorization;
public class OperationsRequirement(string? operation = null) : IAuthorizationRequirement
{
    public string? Operation { get; private set; } = operation;
}
public class OwnerRequirement : IAuthorizationRequirement { }