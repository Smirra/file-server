using Identity.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("admin")]
[Authorize(Roles = "Admin")]
public class AdminController(IIdentityApiRepository identityApiRepository) : ControllerBase
{
    private readonly IIdentityApiRepository _identityApiRepository = identityApiRepository;

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(string? query, string? sortOrder, int pageIndex = 1, int pageSize = 10)
    {
        try
        {
            var users = await _identityApiRepository.GetUsers(query, sortOrder, pageIndex, pageSize);

            return Ok(new
            {
                users,
                paging = new { users.PageIndex, users.PageSize, users.TotalPages, users.TotalCount, users.HasPreviousPage, users.HasNextPage }
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}