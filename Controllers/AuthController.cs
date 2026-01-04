using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CryptoService.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("token")]
    public IActionResult Token()
    {
        var jwt = _config.GetSection("Jwt");
        var signingKeyString = jwt["SigningKey"];
        if (string.IsNullOrWhiteSpace(signingKeyString))
        {
            throw new InvalidOperationException("JWT SigningKey is missing or empty. Check appsettings.json.");
        }
        if (!int.TryParse(jwt["TokenLifetimeMinutes"], out var TokenLifetimeMinutes))
        {
            throw new InvalidOperationException("JWT TokenLifetimeMinutes must be a valid integer");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKeyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "demo-user"),
            new Claim(ClaimTypes.Role, "crypto-user")
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(TokenLifetimeMinutes),
            signingCredentials: creds
        );

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}