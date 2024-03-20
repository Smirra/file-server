using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace Files.Api.Services;
public class JwtService(IConfiguration config, IHttpContextAccessor httpContextAccessor) : IJwtService
{
    public JwtSecurityToken GenerateToken()
    {
        var principal = httpContextAccessor.HttpContext?.User;

        var privateKeyPath = config["JWT_PRIVATEKEYPATH"];
        var privateKey = File.ReadAllText(privateKeyPath
                                             ?? throw new IOException("Private key not found."));
        var rsaPrivateKey = RSA.Create();
        rsaPrivateKey.ImportFromPem(privateKey);

        var key = new RsaSecurityKey(rsaPrivateKey); 
        var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature);

        var token = new JwtSecurityToken(
            config["JWT_ISSUER"],
            config["JWT_AUDIENCE"],
            principal?.Claims,
            expires: DateTime.UtcNow.AddMinutes(1), // Token expiration time
            signingCredentials: creds
        );

        return token;
    }
}

public interface IJwtService {
    JwtSecurityToken GenerateToken();
}