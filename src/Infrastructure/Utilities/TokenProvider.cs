using System.Security.Claims;
using System.Text;
using Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Utilities;


public class TokenProvider : ITokenProvider
{
    private readonly IConfiguration _configuration;

    public TokenProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Create(Guid userId)
    {
        string secretKey = _configuration["Authentication:JwtSecret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Role, "user")
                }),
            Expires = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("Authentication:ExpirationInHours")),
            SigningCredentials = credentials,
            Issuer = _configuration["Authentication:Issuer"],
            Audience = _configuration["Authentication:Audience"]
        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}
