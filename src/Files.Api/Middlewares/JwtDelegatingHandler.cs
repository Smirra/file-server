using Files.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Files.Api.Middlewares;
public class JwtDelegatingHandler(IHttpContextAccessor httpContextAccessor, IJwtService jwtService) : DelegatingHandler
{
    private JwtSecurityToken? cachedToken;
    private AuthenticationHeaderValue? cachedAuthHeader;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!IsTokenValid())
        {
            cachedToken = jwtService.GenerateToken();
            cachedAuthHeader = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme,
                                                             new JwtSecurityTokenHandler().WriteToken(cachedToken));
        }

        request.Headers.Authorization = cachedAuthHeader; 
        return await base.SendAsync(request, cancellationToken);
    }

    private bool IsTokenValid()
    {
        var currentUserIdentifier = httpContextAccessor.HttpContext?.User.Claims
                                    .FirstOrDefault(c => c.Type
                                                         == ClaimTypes.NameIdentifier)?.Value;
        var jwtUser = cachedToken?.Claims
                      .FirstOrDefault(c => c.Type
                                           == ClaimTypes.NameIdentifier)?.Value; 
        if (cachedToken == null || currentUserIdentifier != jwtUser)
            return false;

        return cachedToken.ValidTo > DateTime.UtcNow;
    }
}